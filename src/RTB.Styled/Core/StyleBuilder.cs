using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    public sealed class StyleBuilder : IStyleBuilder, IStyleSnapshot
    {
        private static readonly ObjectPool<StringBuilder> _sbPool =
            new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy
            {   
                InitialCapacity = 256,
                MaximumRetainedCapacity = 1024
            });

        private readonly List<IStyleContributor> _children = [];
        private readonly Dictionary<string, string> _props = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Type, IStyleModule> _modules = [];

        public static StyleBuilder Start => new();

        // IStyleSnapshot
        IReadOnlyDictionary<string, string> IStyleSnapshot.Props => _props;
        IReadOnlyDictionary<Type, IStyleModule> IStyleSnapshot.Modules => _modules;
        IEnumerable<IStyleContributor> IStyleSnapshot.Children => _children;

        public void Register(IStyleContributor child) => _children.Add(child);
        public void Unregister(IStyleContributor child) => _children.Remove(child);

        public IStyleBuilder Append(string property, string value)
        {
            if (string.IsNullOrWhiteSpace(property)) return this;
            if (string.IsNullOrWhiteSpace(value)) return this;
            _props[property.Trim()] = value;
            return this;
        }

        public IStyleBuilder AppendIf(string property, string? value, bool condition)
        {
            if (!condition) return this;
            // property must be valid; value may be null/empty → ignore
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value)) return this;
            _props[property.Trim()] = value!;
            return this;
        }

        public IStyleBuilder AppendIfNotNull(string property, string? value)
            => AppendIf(property, value, !string.IsNullOrWhiteSpace(value));

        public IStyleBuilder Join(params IStyleBuilder[] others)
        {
            if (others is null || others.Length == 0) return this;

            foreach (var other in others)
            {
                if (other is not IStyleSnapshot snap) continue;

                // props: last write wins
                foreach (var kv in snap.Props)
                    _props[kv.Key] = kv.Value;

                // modules: merge generically by type
                foreach (var kv in snap.Modules)
                {
                    var mine = GetOrAddModule(kv.Key);
                    mine.JoinFrom(kv.Value);
                }

                // children: carry over
                foreach (var c in snap.Children)
                    if (!_children.Contains(c))
                        _children.Add(c);
            }
            return this;
        }

        public string Build()
        {
            var sb = _sbPool.Get();
            try
            {
                foreach (var child in _children)
                    if (child.Condition) child.Contribute(this);

                // Inside-blocks
                sb.Append('{');
                foreach (var kv in _props)
                {
                    sb.Append(kv.Key).Append(':').Append(kv.Value).Append(';'); // no extra allocs
                }
                foreach (var m in _modules.Values)
                {
                    if (m.HasInside) m.Build(sb);
                }
                sb.Append('}');

                // Outside blocks
                foreach (var m in _modules.Values)
                    if (m.HasOutside) m.Build(sb);

                return sb.ToString().Trim();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error building style: {ex.Message}");
                return string.Empty;
            }
            finally
            {
                Clear();
                _sbPool.Return(sb);
            }
        }

        public void Clear()
        {
            _props.Clear();
            foreach (var m in _modules.Values) m.Clear();
        }

        // Generic (non-generic) registry access for extensibility
        private IStyleModule GetOrAddModule(Type t)
        {
            if (_modules.TryGetValue(t, out var existing)) return existing;
            var created = (IStyleModule?)Activator.CreateInstance(t)
                          ?? throw new InvalidOperationException($"Cannot create module {t.Name}");
            _modules.Add(t, created);
            return created;
        }

        public T GetOrAddModule<T>() where T : class, IStyleModule, new()
            => (T)GetOrAddModule(typeof(T));
    }
}