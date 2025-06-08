using System;
using System.Linq;
using System.Text;
using RTB.BlazorUI.Styles;
using System.Buffers;
using Microsoft.Extensions.ObjectPool;

namespace RTB.BlazorUI.Styles
{
    /// <summary>
    /// A fluent builder for constructing inline CSS style strings.
    /// </summary>
    public class StyleBuilder
    {
        private static readonly ObjectPool<StringBuilder> _stringBuilderPool = new StringBuilderPool();

        private readonly List<StyleAction> _actions = new();
        private string? _cachedResult;

        private StyleBuilder(string? initStyle = null)
        {
            if (!string.IsNullOrEmpty(initStyle))
            {
                _actions.Add(new RawStyleAction(initStyle));
            }
        }

        public void Clear()
        {
            _actions.Clear();
            _cachedResult = null;
        }

        /// <summary>
        /// Gets a new instance of StyleBuilder.
        /// </summary>
        public static StyleBuilder Start => new();

        /// <summary>
        /// Creates a new instance of StyleBuilder with optional initial styles.
        /// </summary>
        /// <param name="initStyles">Initial style strings to start with.</param>
        /// <returns>A new instance of StyleBuilder.</returns>
        public static StyleBuilder Create(params string?[]? initStyles)
        {
            if (initStyles is null || initStyles.Length == 0)
                return new StyleBuilder();

            var validStyles = initStyles.Where(s => !string.IsNullOrWhiteSpace(s));
            return new StyleBuilder(string.Join(" ", validStyles));
        }

        /// <summary>
        /// Appends a CSS property with its value to the builder.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder Append(string? property, string? value)
        {
            if (string.IsNullOrWhiteSpace(property) || string.IsNullOrWhiteSpace(value))
                return this;

            _actions.Add(new PropertyAction(property, value));

            return this;
        }

        /// <summary>
        /// Conditionally appends a CSS property with its value if the condition is true.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendIf(string property, string? value, bool condition)
        {
            if (condition && !string.IsNullOrWhiteSpace(value))
            {
                _actions.Add(new PropertyAction(property, value));
    
            }
            return this;
        }

        /// <summary>
        /// Conditionally appends a CSS property with its value if the condition function returns true.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <param name="condition">The condition function to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendIf(string property, string? value, Func<bool> condition)
        {
            _actions.Add(new ConditionalPropertyAction(property, value, condition));

            return this;
        }

        /// <summary>
        /// Appends a raw CSS style string to the builder.
        /// </summary>
        /// <param name="style">The CSS style string to append.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendStyle(string? style)
        {
            if (string.IsNullOrWhiteSpace(style))
                return this;

            _actions.Add(new RawStyleAction(style));

            return this;
        }

        /// <summary>
        /// Appends a <see cref="IStyle"/> style to the builder.
        /// </summary>
        /// <param name="style">The CSS style string to append.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendStyle(IStyle? style)
        {
            if (style is null)
                return this;

            _actions.Add(new StyleObjectAction(style));

            return this;
        }

        /// <summary>
        /// Conditionally appends a raw CSS style string if the condition is true.
        /// </summary>
        /// <param name="style">The CSS style string to append.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendStyleIf(string? style, bool condition)
        {
            if (condition && !string.IsNullOrWhiteSpace(style))
            {
                _actions.Add(new RawStyleAction(style));
            }
            return this;
        }

        /// <summary>
        /// Conditionally appends a raw CSS style string if the condition function returns true.
        /// </summary>
        /// <param name="style">The CSS style string to append.</param>
        /// <param name="condition">The condition function to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendStyleIf(string? style, Func<bool> condition)
        {
            _actions.Add(new ConditionalRawStyleAction(style, condition));

            return this;
        }

        /// <summary>
        /// Joins another StyleBuilder's content to this builder.
        /// </summary>
        /// <param name="other">The other StyleBuilder to join.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder Append(StyleBuilder? other)
        {
            if (other is null) 
                return this;

            _actions.AddRange(other._actions);

            return this;
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
                foreach (var action in _actions)
                {
                    action.Apply(builder);
                }

                _cachedResult = builder.ToString().Trim();
                return _cachedResult;
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.Error.WriteLine($"Error building style: {ex.Message}");
                _cachedResult = string.Empty;
                return _cachedResult;
            }
            finally
            {
                _stringBuilderPool.Return(builder);
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

        /// <summary>
        /// Action abstractions for deferred execution
        /// </summary>
        private abstract class StyleAction
        {
            public virtual void Apply(StringBuilder builder)
            {
                if (builder.Length > 0)
                {
                    char lastChar = builder[builder.Length - 1];
                    if (lastChar != ';' && lastChar != ' ')
                        builder.Append(' ');
                }
            }
        }

        private class PropertyAction : StyleAction
        {
            private readonly string _property;
            private readonly string _value;

            public PropertyAction(string property, string value)
            {
                _property = property;
                _value = value;
            }

            public override void Apply(StringBuilder builder)
            {
                base.Apply(builder);

                builder.Append(_property).Append(':').Append(_value).Append(';');
            }
        }

        private class ConditionalPropertyAction : StyleAction
        {
            private readonly string _property;
            private readonly string? _value;
            private readonly Func<bool> _condition;

            public ConditionalPropertyAction(string property, string? value, Func<bool> condition)
            {
                _property = property;
                _value = value;
                _condition = condition;
            }

            public override void Apply(StringBuilder builder)
            {
                if (_condition() && !string.IsNullOrWhiteSpace(_value))
                {
                    base.Apply(builder);

                    builder.Append(_property).Append(':').Append(_value).Append(';');
                }
            }
        }

        private class RawStyleAction : StyleAction
        {
            private readonly string _style;

            public RawStyleAction(string style)
            {
                _style = style.Trim();
            }

            public override void Apply(StringBuilder builder)
            {
                base.Apply(builder);

                builder.Append(_style);
            }
        }

        private class ConditionalRawStyleAction : StyleAction
        {
            private readonly string? _style;
            private readonly Func<bool> _condition;

            public ConditionalRawStyleAction(string? style, Func<bool> condition)
            {
                _style = style?.Trim();
                _condition = condition;
            }

            public override void Apply(StringBuilder builder)
            {
                if (_condition() && !string.IsNullOrWhiteSpace(_style))
                {
                    base.Apply(builder);

                    builder.Append(_style);
                }
            }
        }

        private class StyleObjectAction : StyleAction
        {
            private readonly IStyle _style;

            public StyleObjectAction(IStyle style)
            {
                _style = style;
            }

            public override void Apply(StringBuilder builder)
            {
                var styleString = _style.ToStyle()?.Build()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(styleString)) return;

                base.Apply(builder);

                builder.Append(styleString);
            }
        }
    }

    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        public override StringBuilder Get() => new StringBuilder(256);
        
        public override void Return(StringBuilder obj)
        {
            if (obj.Capacity > 1024) return; // Don't pool large builders
            obj.Clear();
        }
    }
}
