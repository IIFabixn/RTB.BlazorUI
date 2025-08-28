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

namespace RTB.Blazor.Styled
{
    /// <summary>
    /// A fluent builder for constructing inline CSS style strings.
    /// </summary>
    public class StyleBuilder
    {
        private static readonly ObjectPool<StringBuilder> _stringBuilderPool = new StringBuilderPool();

        private readonly List<RTBStyleBase> _children = [];

        private readonly Dictionary<string, string> _props = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _selectors = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _medias = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _animations = new(StringComparer.OrdinalIgnoreCase);

        public void Clear()
        {
            _props.Clear();
            _selectors.Clear();
            _medias.Clear();
            _animations.Clear();
        }

        public void Register(RTBStyleBase child)
        {
            _children.Add(child);
        }
        public void Unregister(RTBStyleBase child)
        {
            _children.Remove(child);
        }

        /// <summary>
        /// Gets a new instance of StyleBuilder.
        /// </summary>
        public static StyleBuilder Start => new();

        /// <summary>
        /// Appends a CSS property with its value to the builder.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder Append(string property, string value)
        {
            return AppendInternal(property, value);
        }

        /// <summary>
        /// Conditionally appends a CSS property with its value if the condition is true.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendIf(string? property, string? value, bool condition)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value) || !condition) return this;

            return AppendInternal(property, value);
        }

        /// <summary>
        /// Conditionally appends a CSS property with its value if the value is not null or whitespace.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendIfNotNull(string property, string? value)
        {
            return AppendIf(property, value, !string.IsNullOrWhiteSpace(value));
        }

        public StyleBuilder Join(params StyleBuilder[] others)
        {
            if (others is { Length: 0 })
                return this;

            foreach (var other in others)
            {
                // Append properties from the other StyleBuilder
                foreach (var kvp in other._props)
                {
                    AppendInternal(kvp.Key, kvp.Value);
                }

                // Append selectors from the other StyleBuilder
                foreach (var kvp in other._selectors)
                {
                    AppendSelector(kvp.Key, kvp.Value);
                }

                // Append media queries from the other StyleBuilder
                foreach (var kvp in other._medias)
                {
                    AppendMedia(new BreakPoint { Media = BreakPoint.MediaType.Screen }, kvp.Value); // Assuming default media type
                }
            }
            return this;
        }

        public StyleBuilder AppendSelector(string selector, string style)
        {
            if (string.IsNullOrWhiteSpace(selector) || string.IsNullOrWhiteSpace(style))
                return this;

            var key = selector.Trim();
            if (!_selectors.TryAdd(key, style))
            {
                // If the selector already exists, update its value
                _selectors[key] = style;
            }

            return this;
        }

        public StyleBuilder AppendAnimation(string name, string frames)
        {
            if (string.IsNullOrEmpty(name))
                return this;

            var key = name.Trim();
            if (!_animations.TryAdd(key, frames))
            {
                // If the animation already exists, update its value
                _animations[key] = frames;
            }

            return this;
        }

        public StyleBuilder AppendKeyFrame(string animationName, string offset, string frame)
        {
            if (string.IsNullOrEmpty(animationName) || string.IsNullOrWhiteSpace(offset) || string.IsNullOrWhiteSpace(frame))
                return this;

            var key = animationName.Trim();
            var frameContent = $"{offset.Trim()}{{{frame}}}";
            if (!_animations.TryAdd(key, frameContent))
            {
                // If the animation already exists, update its value
                _animations[key] += ' ' + frameContent;
            }

            return this;
        }

        public StyleBuilder AppendMedia(BreakPoint media, string style)
        {
            if (media == null || string.IsNullOrWhiteSpace(style))
                return this;

            // Create a media query selector
            string mediaQuery = media.ToQuery();
            if (!_medias.TryAdd(mediaQuery, style))
            {
                // If the media query already exists, update its value
                _medias[mediaQuery] = style;
            }

            return this;
        }

        /// <summary>
        /// Builds the final CSS style string.<br/>
        /// Resets the dirty state after building.
        /// </summary>
        /// <returns>The constructed CSS style string.</returns>
        public string Build()
        {
            var builder = _stringBuilderPool.Get();
            foreach(var child in _children.Where(c => c.Condition))
            {
                child.BuildStyle(this);
            }

            try
            {
                builder.Append('{');
                // Apply each action to the builder
                foreach (var prop in _props)
                {
                    builder.Append($"{prop.Key}:{prop.Value};");
                }

                // Append selectors if any
                foreach (var sel in _selectors)
                {
                    // sel value alredy comes with braces, since it's also just a regular css but wrapped in query
                    builder.Append($"{sel.Key}{sel.Value}"); // Append nested selector
                }

                builder.Append('}');

                // Append media queries if any
                foreach (var media in _medias)
                {
                    builder.Append($"{media.Key}{{{media.Value}}}");
                }

                foreach(var animation in _animations)
                {
                    builder.Append($"@keyframes {animation.Key}{{{animation.Value}}}");
                }

                var css = builder.ToString().Trim();

                return css;
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.Error.WriteLine($"Error building style: {ex.Message}");
                return string.Empty;
            }
            finally
            {
                Clear(); // Reset the state before building
                _stringBuilderPool.Return(builder);
            }
        }

        private StyleBuilder AppendInternal(string property, string value)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
                return this;

            var key = property.Trim();

            if (!_props.TryAdd(key, value))
            {
                // If the property already exists, update its value, latest one wins
                _props[key] = value;
            }

            return this;
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
