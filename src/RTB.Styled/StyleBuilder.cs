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
    /// <summary>
    /// A builder interface for constructing CSS style strings with support for conditional properties, selectors, media queries, and animations.
    /// </summary>
    public interface IStyleBuilder
    {
        /// <summary>
        /// Appends a CSS property and its value to the style.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IStyleBuilder Append(string property, string value);

        /// <summary>
        /// Appends a CSS property and its value to the style only if the specified condition is true.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        IStyleBuilder AppendIf(string? property, string? value, bool condition);

        /// <summary>
        /// Appends a CSS property and its value to the style only if the value is not null or whitespace.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IStyleBuilder AppendIfNotNull(string property, string? value);

        /// <summary>
        /// Merges another StyleBuilder into this one, combining properties and modules.
        /// </summary>
        /// <param name="others"></param>
        /// <returns></returns>
        IStyleBuilder Join(params IStyleBuilder[] others);

        /// <summary>
        /// Builds the final CSS style string, incorporating properties, selectors, media queries, and animations.
        /// </summary>
        /// <returns></returns>
        string Build();

        /// <summary>
        /// Registers a child style component to contribute to the final style.
        /// </summary>
        /// <param name="child"></param>
        void Register(RTBStyleBase child);

        /// <summary>
        /// Unregisters a child style component.
        /// </summary>
        /// <param name="child"></param>
        void Unregister(RTBStyleBase child);
    }

    /// <summary>
    /// Internal contract for pluggable style modules.
    /// Selectors contribute inside the main block; media/animations render outside.
    /// </summary>
    public interface IStyleModule
    {
        /// <summary>
        /// Indicates if the module has content to render inside the main style block.
        /// </summary>
        bool HasInside { get; }
        /// <summary>
        /// Indicates if the module has content to render outside the main style block.
        /// </summary>
        bool HasOutside { get; }

        /// <summary>
        /// Builds the module's content to be included inside the main style block.
        /// </summary>
        /// <param name="sb"></param>
        void Build(StringBuilder sb);

        /// <summary>
        /// Clears all content from the module.
        /// </summary>
        void Clear();

        /// <summary>Merge content from another instance of the same module.</summary>
        void JoinFrom(IStyleModule other);
    }

    /// <summary>
    /// Internal contract for the core to host modules.
    /// </summaryIStyleBuilder
    internal interface IModuleHost
    {
        /// <summary>
        /// Gets an existing module of type T or adds a new one if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetOrAddModule<T>() where T : class, IStyleModule, new();

        /// <summary>
        /// Tries to get an existing module of type T. Returns null if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T? TryGetModule<T>() where T : class, IStyleModule;
    }

    /// <summary>
    /// A builder for CSS style strings with support for conditional properties, selectors, media queries, and animations.
    /// </summary>
    public class StyleBuilder : IStyleBuilder, IModuleHost
    {
        /// <summary>
        /// A pool of StringBuilder instances to minimize allocations during style building.
        /// </summary>
        private static readonly ObjectPool<StringBuilder> _stringBuilderPool = new StringBuilderPool();

        /// <summary>
        /// Registered child style components that can contribute to the final style.
        /// </summary>
        private readonly List<RTBStyleBase> _children = [];

        /// <summary>
        /// A dictionary to hold CSS properties and their values.
        /// </summary>
        private readonly Dictionary<string, string> _props = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A dictionary to hold style modules by their type.
        /// </summary>
        private readonly Dictionary<Type, IStyleModule> _modules = [];

        /// <summary>
        /// Starts a new StyleBuilder instance.
        /// </summary>
        public static StyleBuilder Start => new();

        /// <summary>
        /// Registers a child style component to contribute to the final style.
        /// </summary>
        /// <param name="child"></param>
        public void Register(RTBStyleBase child) => _children.Add(child);

        /// <summary>
        /// Unregisters a child style component.
        /// </summary>
        /// <param name="child"></param>
        public void Unregister(RTBStyleBase child) => _children.Remove(child);

        /// <summary>
        /// Appends a CSS property and its value to the style.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IStyleBuilder Append(string property, string value) => AppendInternal(property, value);

        /// <summary>
        /// Appends a CSS property and its value to the style only if the specified condition is true.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IStyleBuilder AppendIf(string? property, string? value, bool condition)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value) || !condition) return this;
            return AppendInternal(property, value);
        }

        /// <summary>
        /// Appends a CSS property and its value to the style only if the value is not null or whitespace.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IStyleBuilder AppendIfNotNull(string property, string? value)
            => AppendIf(property, value, !string.IsNullOrWhiteSpace(value));

        /// <summary>
        /// Merges another StyleBuilder into this one, combining properties and modules.
        /// </summary>
        /// <param name="others"></param>
        /// <returns></returns>
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

            // Local function to merge a specific module type
            void MergeModule<T>(StyleBuilder otherBuilder) where T : class, IStyleModule, new()
            {
                var otherModule = otherBuilder.TryGetModule<T>();
                if (otherModule is null) return;

                var myModule = GetOrAddModule<T>();
                myModule.JoinFrom(otherModule);
            }
        }

        /// <summary>
        /// Builds the final CSS style string, incorporating properties, selectors, media queries, and animations.
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            var sb = _stringBuilderPool.Get();

            try
            {
                // let children contribute before final build
                foreach (var child in _children.Where(c => c.Condition))
                    child.BuildStyle(this);

                // gather modules
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

                    // selectors render inside the same block https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_nesting/Using_CSS_nesting
                    selectors?.Build(sb);

                    sb.Append('}');
                }

                // media queries outside
                medias?.Build(sb);

                // keyframes outside
                anims?.Build(sb);

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
        /// <summary>
        /// Gets an existing module of type T or adds a new one if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOrAddModule<T>() where T : class, IStyleModule, new()
        {
            var t = typeof(T);
            if (_modules.TryGetValue(t, out var existing)) return (T)existing;
            var m = new T();
            _modules.Add(t, m);
            return m;
        }

        /// <summary>
        /// Tries to get an existing module of type T. Returns null if not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? TryGetModule<T>() where T : class, IStyleModule
            => _modules.TryGetValue(typeof(T), out var existing) ? (T)existing : null;
        #endregion

        /// <summary>
        /// Clears all properties and modules to reset the builder state.
        /// </summary>
        private void ClearCore()
        {
            _props.Clear();
            foreach (var m in _modules.Values) m.Clear();
        }

        /// <summary>
        /// Internal method to append a property and its value, ensuring they are not null or whitespace.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private StyleBuilder AppendInternal(string property, string value)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
                return this;

            _props[property.Trim()] = value;
            return this;
        }
    }

    /// <summary>
    /// A style module to manage CSS selectors and their associated declarations.
    /// </summary>
    internal sealed class SelectorModule : IStyleModule
    {
        /// <summary>
        /// A dictionary to hold selectors and their corresponding declarations.
        /// </summary>
        private readonly Dictionary<string, string> _selectors = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Indicates if there are any selectors to render inside the main style block.
        /// </summary>
        public bool HasInside => _selectors.Count > 0;

        /// <summary>
        /// Indicates that selectors do not render outside the main style block.
        /// </summary>
        public bool HasOutside => false;

        /// <summary>
        /// Appends a selector and its declarations to the module.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="declarations"></param>
        public void Append(string selector, string declarations)
        {
            if (string.IsNullOrWhiteSpace(selector) || string.IsNullOrWhiteSpace(declarations)) return;
            var key = selector.Trim();
            if (!_selectors.TryAdd(key, declarations)) _selectors[key] += declarations;
        }

        /// <summary>
        /// Builds the selectors and their declarations inside the main style block.
        /// </summary>
        /// <param name="sb"></param>
        public void Build(StringBuilder sb)
        {
            foreach (var sel in _selectors)
                sb.Append(sel.Key).Append(sel.Value);
        }
        /// <summary>
        /// Clears all selectors and their declarations.
        /// </summary>
        public void Clear() => _selectors.Clear();

        /// <summary>
        /// Merges selectors and their declarations from another SelectorModule instance.
        /// </summary>
        /// <param name="other"></param>
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

    /// <summary>
    /// A style module to manage CSS media queries and their associated inner styles.
    /// </summary>
    internal sealed class MediaModule : IStyleModule
    {
        /// <summary>
        /// A dictionary to hold media query conditions and their corresponding inner styles.
        /// </summary>
        private readonly Dictionary<string, string> _medias = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Indicates that media queries do not render inside the main style block.
        /// </summary>
        public bool HasInside => false;

        /// <summary>
        /// Indicates if there are any media queries to render outside the main style block.
        /// </summary>
        public bool HasOutside => _medias.Count > 0;

        /// <summary>
        /// Appends a media query condition and its inner styles to the module.
        /// </summary>
        /// <param name="media"></param>
        /// <param name="inner"></param>
        public void Append(string media, string inner)
        {
            if (media == null || string.IsNullOrWhiteSpace(inner)) return;
            if (!_medias.TryAdd(media, inner)) _medias[media] += inner;
        }

        /// <summary>
        /// Builds the media queries and their inner styles outside the main style block.
        /// </summary>
        /// <param name="sb"></param>
        public void Build(StringBuilder sb)
        {
            foreach (var m in _medias)
                sb.Append(m.Key).Append(m.Value);
        }

        /// <summary>
        /// Clears all media queries and their inner styles.
        /// </summary>
        public void Clear() => _medias.Clear();

        /// <summary>
        /// Merges media queries and their inner styles from another MediaModule instance.
        /// </summary>
        /// <param name="other"></param>
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

    /// <summary>
    /// A style module to manage CSS animations and their associated keyframes.
    /// </summary>
    internal sealed class AnimationModule : IStyleModule
    {
        /// <summary>
        /// A dictionary to hold animation names and their corresponding keyframe definitions.
        /// </summary>
        private readonly Dictionary<string, string> _animations = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Indicates that animations do not render inside the main style block.
        /// </summary>
        public bool HasInside => false;

        /// <summary>
        /// Indicates if there are any animations to render outside the main style block.
        /// </summary>
        public bool HasOutside => _animations.Count > 0;

        /// <summary>
        /// Ensures that an animation with the specified name exists in the module.
        /// </summary>
        /// <param name="name"></param>
        public void EnsureAnimation(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _animations.TryAdd(name.Trim(), string.Empty);
        }

        /// <summary>
        /// Appends a keyframe definition to the specified animation.
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="offset"></param>
        /// <param name="frame"></param>
        public void AppendFrame(string animationName, string offset, string frame)
        {
            if (string.IsNullOrWhiteSpace(animationName) || string.IsNullOrWhiteSpace(offset) || string.IsNullOrWhiteSpace(frame))
                return;

            var key = animationName.Trim();
            var frameContent = $" {offset.Trim()}{frame}";
            if (!_animations.TryAdd(key, frameContent))
                _animations[key] += frameContent;
        }

        /// <summary>
        /// Builds the keyframe definitions for all animations outside the main style block.
        /// </summary>
        /// <param name="sb"></param>
        public void Build(StringBuilder sb)
        {
            foreach (var a in _animations)
                sb.Append("@keyframes ").Append(a.Key).Append(" { ").Append(a.Value).Append(" }");
        }

        /// <summary>
        /// Clears all animations and their keyframe definitions.
        /// </summary>
        public void Clear() => _animations.Clear();

        /// <summary>
        /// Merges animations and their keyframe definitions from another AnimationModule instance.
        /// </summary>
        /// <param name="other"></param>
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

    /// <summary>
    /// Extension methods for IStyleBuilder to support selectors, media queries, and animations.
    /// </summary>
    public static class StyleBuilderSelectorExtensions
    {
        /// <summary>
        /// Appends a CSS selector and its declarations to the style builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="selector"></param>
        /// <param name="declarations"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IStyleBuilder AppendSelector(this IStyleBuilder builder, string selector, string declarations)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<SelectorModule>().Append(selector, declarations);
            return builder;
        }
    }

    /// <summary>
    /// Extension methods for IStyleBuilder to support media queries.
    /// </summary>
    public static class StyleBuilderMediaExtensions
    {
        /// <summary>
        /// Appends a CSS media query and its inner styles to the style builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="media"></param>
        /// <param name="inner"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IStyleBuilder AppendMedia(this IStyleBuilder builder, string media, string inner)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<MediaModule>().Append(media, inner);
            return builder;
        }
    }

    /// <summary>
    /// Extension methods for IStyleBuilder to support CSS animations and keyframes.
    /// </summary>
    public static class StyleBuilderAnimationExtensions
    {
        /// <summary>
        /// Appends a CSS animation by name to the style builder, ensuring it exists.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IStyleBuilder AppendAnimation(this IStyleBuilder builder, string name)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<AnimationModule>().EnsureAnimation(name);
            return builder;
        }

        /// <summary>
        /// Appends a keyframe definition to a named CSS animation in the style builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="animationName"></param>
        /// <param name="offset"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IStyleBuilder AppendKeyFrame(this IStyleBuilder builder, string animationName, string offset, string frame)
        {
            if (builder is not IModuleHost host) throw new InvalidOperationException("Builder does not support modules.");
            host.GetOrAddModule<AnimationModule>().AppendFrame(animationName, offset, frame);
            return builder;
        }
    }

    /// <summary>
    /// A simple object pool for StringBuilder instances to reduce allocations.
    /// </summary>
    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        /// <summary>
        /// A concurrent bag to hold pooled StringBuilder instances.
        /// </summary>
        private readonly ConcurrentBag<StringBuilder> _bag = [];
        /// <summary>
        /// Gets a StringBuilder from the pool or creates a new one if the pool is empty.
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Get() => _bag.TryTake(out var sb) ? sb : new StringBuilder(256);

        /// <summary>
        /// Returns a StringBuilder to the pool after clearing its content.
        /// </summary>
        /// <param name="obj"></param>
        public override void Return(StringBuilder obj)
        {
            if (obj.Capacity > 1024) return; // Don't pool large builders
            obj.Clear();
            _bag.Add(obj);
        }
    }
}
