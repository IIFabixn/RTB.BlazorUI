using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace RTB.Blazor.Styled.Services
{
    /// <summary>
    /// Registry for scoped CSS: C# produces fully-scoped text, JS only clears+appends for a class.
    /// </summary>
    public interface IStyleRegistry
    {
        /// <summary>Acquire (or create) a stable class name. If null, a GUID-based name is generated.</summary>
        string Acquire(string? preferredClassName = null);

        /// <summary>Insert/update the fully-scoped CSS for the class. No-op if CSS unchanged.</summary>
        ValueTask UpsertScopedAsync(string scopedCss, string className);

        /// <summary>Release one reference; when it reaches zero, the rules are cleared and the class entry is removed.</summary>
        ValueTask<bool> Release(string className);

        /// <summary>Clear all injected styles and reset the registry.</summary>
        ValueTask ClearAll();

        /// <summary>Helper to generate a short, stable-looking class name.</summary>
        string GenerateClassName(string prefix = "rtb-");
    }

    public sealed class StyleRegistry(IJSRuntime js) : IStyleRegistry
    {
        // className -> entry
        private readonly ConcurrentDictionary<string, Entry> _entries =
            new(StringComparer.Ordinal);

        private sealed class Entry
        {
            public int RefCount;
            public string? LastCss;
            public readonly SemaphoreSlim Gate = new(1, 1);
        }

        public string GenerateClassName(string prefix = "rtb-")
            => $"{prefix}{Guid.NewGuid():N}";

        public string Acquire(string? preferredClassName = null)
        {
            var cls = string.IsNullOrWhiteSpace(preferredClassName)
                      ? GenerateClassName("rtb-")
                      : preferredClassName.Trim();

            _entries.AddOrUpdate(
                cls,
                _ => new Entry { RefCount = 1 },
                (_, e) => { Interlocked.Increment(ref e.RefCount); return e; });

            return cls;
        }

        public async ValueTask UpsertScopedAsync(string scopedCss, string className)
        {
            if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(scopedCss))
                return;

            if (!_entries.TryGetValue(className, out var entry))
            {
                // If the caller forgot to Acquire, create a single-ref entry so we can inject.
                entry = _entries.GetOrAdd(className, _ => new Entry { RefCount = 1 });
            }

            await entry.Gate.WaitAsync().ConfigureAwait(false);
            try
            {
                if (string.Equals(entry.LastCss, scopedCss, StringComparison.Ordinal))
                    return; // unchanged

                // One-shot JS that clears .className and appends the given scoped CSS text
                await js.InvokeVoidAsync("rtbStyled.injectScoped", scopedCss, className);

                entry.LastCss = scopedCss;
            }
            finally
            {
                entry.Gate.Release();
            }
        }

        public async ValueTask<bool> Release(string className)
        {
            if (string.IsNullOrWhiteSpace(className)) return false;
            if (!_entries.TryGetValue(className, out var entry)) return false;

            // dec ref
            while (true)
            {
                var cur = Volatile.Read(ref entry.RefCount);
                if (cur <= 0) return false;
                if (Interlocked.CompareExchange(ref entry.RefCount, cur - 1, cur) == cur)
                    break;
            }

            if (Volatile.Read(ref entry.RefCount) > 0) return false;

            // last user: remove & clear rules
            _entries.TryRemove(className, out _);

            // Clear is safe even if no rules exist yet
            await js.InvokeVoidAsync("rtbStyled.clearRule", className);
            return true;
        }

        public ValueTask ClearAll()
        {
            _entries.Clear();
            return js.InvokeVoidAsync("rtbStyled.clearAll");
        }
    }
}
