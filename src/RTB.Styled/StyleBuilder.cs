using Microsoft.Extensions.ObjectPool;
using RTB.Blazor.Styled.Helper;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RTB.Blazor.Styled
{
    /// <summary>
    /// A fluent builder for constructing inline CSS style strings.
    /// </summary>
    public partial class StyleBuilder
    {
        private static readonly ObjectPool<StringBuilder> _stringBuilderPool = new StringBuilderPool();

        private readonly Dictionary<string, string> _props = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _selectors = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _medias = new(StringComparer.OrdinalIgnoreCase);
        public bool IsDirty { get; private set; } = false;

        public void Clear()
        {
            _props.Clear();
            _selectors.Clear();
            IsDirty = false;
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
            if (condition && !string.IsNullOrWhiteSpace(property) &&!string.IsNullOrWhiteSpace(value))
                return AppendInternal(property, value);

            return this;
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

            IsDirty = true; // Mark as dirty since we modified the selectors

            return this;
        }

        public StyleBuilder AppendMedia(BreakPoint media, string style)
        {
            if (media == null || string.IsNullOrWhiteSpace(style))
                return this;

            // Create a media query selector
            var mediaQuery = media.ToQuery();
            if (!_medias.TryAdd(mediaQuery, style))
            {
                // If the media query already exists, update its value
                _medias[mediaQuery] = style;
            }

            IsDirty = true; // Mark as dirty since we modified the medias

            return this;
        }

        public StyleBuilder Var(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                return this;
            // CSS variables are prefixed with "--"
            var property = $"--{name.Trim()}";
            return AppendInternal(property, value);
        }

        /// <summary>
        /// Builds the final CSS style string.
        /// </summary>
        /// <returns>The constructed CSS style string.</returns>
        public string Build()
        {
            var builder = _stringBuilderPool.Get();
            try
            {
                // Apply each action to the builder
                foreach (var prop in _props.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
                    builder.Append($"{prop}:{_props[prop]};");

                // Append selectors if any
                foreach (var prop in _selectors)
                    builder.Append($"{prop.Key}{{{prop.Value}}}");

                // Append media queries if any
                foreach (var media in _medias)
                {
                    builder.Append($"{media.Key}{{{media.Value}}}");
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
                _stringBuilderPool.Return(builder);
                IsDirty = false; // Reset dirty state after building
            }
        }

        /// <summary>
        /// Implicitly converts a StyleBuilder to its string representation.
        /// </summary>
        /// <param name="builder">The StyleBuilder to convert.</param>
        public static implicit operator string(StyleBuilder builder)
        {
            return builder?.Build() ?? string.Empty;
        }

        public override string ToString()
        {
            return Build();
        }

        private StyleBuilder AppendInternal(string property, string value)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
                return this;

            var key = property.Trim();

            if (!_props.TryAdd(key, value))
            {
                // If the property already exists, update its value
                _props[key] = value;
            }

            IsDirty = true;

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
