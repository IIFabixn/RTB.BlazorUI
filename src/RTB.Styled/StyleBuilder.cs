using Microsoft.Extensions.ObjectPool;
using RTB.Blazor.Styled.Components;
using RTB.Blazor.Styled.Helper;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace RTB.Blazor.Styled
{
    public interface IStyleBuilder
    {
        IStyleBuilder Append(string property, string value);
        IStyleBuilder AppendIf(string? property, string? value, bool condition);
        IStyleBuilder AppendIfNotNull(string property, string? value);
        IStyleBuilder Join(params IStyleBuilder[] others);
        string Build();

        void Register(RTBStyleBase child);
        void Unregister(RTBStyleBase child);
    }

    /// <summary>
    /// Internal contract for pluggable style modules.
    /// Selectors contribute inside the main block; media/animations render outside.
    /// </summary>
    public interface IStyleModule
    {
        bool HasInside { get; }
        bool HasOutside { get; }

        void BuildInside(StringBuilder sb);
        void BuildOutside(StringBuilder sb);
        void Clear();

        /// <summary>Merge content from another instance of the same module.</summary>
        void JoinFrom(IStyleModule other);
    }

    /// <summary>
    /// Internal contract for the core to host modules.
    /// </summaryIStyleBuilder
    internal interface IModuleHost
    {
        T GetOrAddModule<T>() where T : class, IStyleModule, new();
        T? TryGetModule<T>() where T : class, IStyleModule;
    }
    
    /// <summary>
     /// A fluent builder for constructing inline CSS style strings.
     /// Core handles properties and orchestration; optional modules add features.
     /// </summary>
    public class StyleBuilder : IStyleBuilder, IModuleHost
    {
        private static readonly ObjectPool<StringBuilder> _stringBuilderPool = new StringBuilderPool();

        private readonly List<RTBStyleBase> _children = [];
        private readonly Dictionary<string, string> _props = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Type, IStyleModule> _modules = [];

        public static StyleBuilder Start => new();

        public void Register(RTBStyleBase child) => _children.Add(child);
        public void Unregister(RTBStyleBase child) => _children.Remove(child);

        public IStyleBuilder Append(string property, string value) => AppendInternal(property, value);

        public IStyleBuilder AppendIf(string? property, string? value, bool condition)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value) || !condition) return this;
            return AppendInternal(property, value);
        }

        public IStyleBuilder AppendIfNotNull(string property, string? value)
            => AppendIf(property, value, !string.IsNullOrWhiteSpace(value));

        public IStyleBuilder Join(params IStyleBuilder[] others)
        {
            if (others == null || others.Length == 0) return this;

            foreach (var other in others)
            {
                if (other is not StyleBuilder o) continue;

                // Props: last write wins
                foreach (var kvp in o._props)
                    _props[kvp.Key] = kvp.Value;

                // Modules: merge if present
                MergeModule<SelectorModule>(o);
                MergeModule<MediaModule>(o);
                MergeModule<AnimationModule>(o);
            }

            return this;

            void MergeModule<T>(StyleBuilder otherBuilder) where T : class, IStyleModule, new()
            {
                var otherModule = otherBuilder.TryGetModule<T>();
                if (otherModule is null) return;

                var myModule = GetOrAddModule<T>();
                myModule.JoinFrom(otherModule);
            }
        }

        public string Build()
        {
            var sb = _stringBuilderPool.Get();

            try
            {
                // let children contribute before final build
                foreach (var child in _children.Where(c => c.Condition))
                    child.BuildStyle(this);

                var selectors = TryGetModule<SelectorModule>();
                var medias = TryGetModule<MediaModule>();
                var anims = TryGetModule<AnimationModule>();

                var hasInside = _props.Count > 0 || (selectors?.HasInside ?? false);
                if (hasInside)
                {
                    sb.Append('{');

                    // inline properties
                    foreach (var prop in _props)
                        sb.Append(prop.Key).Append(':').Append(prop.Value).Append(';');

                    // selectors render inside the same block (keeps current behavior)
                    selectors?.BuildInside(sb);

                    sb.Append('}');
                }

                // media queries outside
                medias?.BuildOutside(sb);

                // keyframes outside
                anims?.BuildOutside(sb);

                var css = sb.ToString().Trim();
                return css;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error building style: {ex.Message}");
                return string.Empty;
            }
            finally
            {
                ClearCore();
                _stringBuilderPool.Return(sb);
            }
        }

        #region IModuleHost
        public T GetOrAddModule<T>() where T : class, IStyleModule, new()
        {
            var t = typeof(T);
            if (_modules.TryGetValue(t, out var existing)) return (T)existing;
            var m = new T();
            _modules.Add(t, m);
            return m;
        }

        public T? TryGetModule<T>() where T : class, IStyleModule
            => _modules.TryGetValue(typeof(T), out var existing) ? (T)existing : null;
        #endregion

        private void ClearCore()
        {
            _props.Clear();
            foreach (var m in _modules.Values) m.Clear();
        }

        private StyleBuilder AppendInternal(string property, string value)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
                return this;

            _props[property.Trim()] = value;
            return this;
        }
    }

    internal sealed class SelectorModule : IStyleModule
    {
        private readonly Dictionary<string, string> _selectors = new(StringComparer.OrdinalIgnoreCase);

        public bool HasInside => _selectors.Count > 0;
        public bool HasOutside => false;

        public void Append(string selector, string declarations)
        {
            if (string.IsNullOrWhiteSpace(selector) || string.IsNullOrWhiteSpace(declarations)) return;
            var key = selector.Trim();
            if (!_selectors.TryAdd(key, declarations)) _selectors[key] += declarations;
        }

        public void BuildInside(StringBuilder sb)
        {
            foreach (var sel in _selectors)
                sb.Append(sel.Key).Append(sel.Value);
        }

        public void BuildOutside(StringBuilder sb) { /* nothing */ }

        public void Clear() => _selectors.Clear();

        public void JoinFrom(IStyleModule other)
        {
            if (other is not SelectorModule o) return;
            foreach (var kv in o._selectors)
            {
                if (!_selectors.TryAdd(kv.Key, kv.Value))
                    _selectors[kv.Key] += kv.Value;
            }
        }
    }

    internal sealed class MediaModule : IStyleModule
    {
        private readonly Dictionary<string, string> _medias = new(StringComparer.OrdinalIgnoreCase);

        public bool HasInside => false;
        public bool HasOutside => _medias.Count > 0;

        public void Append(string media, string inner)
        {
            if (media == null || string.IsNullOrWhiteSpace(inner)) return;
            if (!_medias.TryAdd(media, inner)) _medias[media] += inner;
        }

        public void BuildInside(StringBuilder sb) { /* nothing */ }

        public void BuildOutside(StringBuilder sb)
        {
            foreach (var m in _medias)
                sb.Append(m.Key).Append(m.Value);
        }

        public void Clear() => _medias.Clear();

        public void JoinFrom(IStyleModule other)
        {
            if (other is not MediaModule o) return;
            foreach (var kv in o._medias)
            {
                if (!_medias.TryAdd(kv.Key, kv.Value))
                    _medias[kv.Key] += kv.Value;
            }
        }
    }

    internal sealed class AnimationModule : IStyleModule
    {
        private readonly Dictionary<string, string> _animations = new(StringComparer.OrdinalIgnoreCase);

        public bool HasInside => false;
        public bool HasOutside => _animations.Count > 0;

        public void EnsureAnimation(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _animations.TryAdd(name.Trim(), string.Empty);
        }

        public void AppendFrame(string animationName, string offset, string frame)
        {
            if (string.IsNullOrWhiteSpace(animationName) || string.IsNullOrWhiteSpace(offset) || string.IsNullOrWhiteSpace(frame))
                return;

            var key = animationName.Trim();
            var frameContent = $" {offset.Trim()}{frame}";
            if (!_animations.TryAdd(key, frameContent))
                _animations[key] += frameContent;
        }

        public void BuildInside(StringBuilder sb) { /* nothing */ }

        public void BuildOutside(StringBuilder sb)
        {
            foreach (var a in _animations)
                sb.Append("@keyframes ").Append(a.Key).Append(" { ").Append(a.Value).Append(" }");
        }

        public void Clear() => _animations.Clear();

        public void JoinFrom(IStyleModule other)
        {
            if (other is not AnimationModule o) return;
            foreach (var kv in o._animations)
            {
                if (!_animations.TryAdd(kv.Key, kv.Value))
                    _animations[kv.Key] += kv.Value;
            }
        }
    }

    public static class StyleBuilderSelectorExtensions
    {
        public static IStyleBuilder AppendSelector(this IStyleBuilder builder, string selector, string declarations)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<SelectorModule>().Append(selector, declarations);
            return builder;
        }
    }

    public static class StyleBuilderMediaExtensions
    {
        public static IStyleBuilder AppendMedia(this IStyleBuilder builder, string media, string inner)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<MediaModule>().Append(media, inner);
            return builder;
        }
    }

    public static class StyleBuilderAnimationExtensions
    {
        public static IStyleBuilder AppendAnimation(this IStyleBuilder builder, string name)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<AnimationModule>().EnsureAnimation(name);
            return builder;
        }

        public static IStyleBuilder AppendKeyFrame(this IStyleBuilder builder, string animationName, string offset, string frame)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<AnimationModule>().AppendFrame(animationName, offset, frame);
            return builder;
        }
    }

    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        private readonly ConcurrentBag<StringBuilder> _bag = [];
        public override StringBuilder Get() => _bag.TryTake(out var sb) ? sb : new StringBuilder(256);

        public override void Return(StringBuilder obj)
        {
            if (obj.Capacity > 1024) return; // Don't pool large builders
            obj.Clear();
            _bag.Add(obj);
        }
    }
}
