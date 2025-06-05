using System;
using System.Linq;
using System.Text;
using RTB.BlazorUI.Styles;

namespace RTB.BlazorUI.Helper
{
    /// <summary>
    /// A fluent builder for constructing inline CSS style strings.
    /// </summary>
    public class StyleBuilder
    {
        private readonly StringBuilder _builder;

        private StyleBuilder(string? initStyle = null)
        {
            _builder = new StringBuilder(initStyle ?? string.Empty);
        }

        /// <summary>
        /// Gets a new instance of StyleBuilder.
        /// </summary>
        public static StyleBuilder Start => new();

        /// <summary>
        /// Clears all styles from the builder.
        /// </summary>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder Clear()
        {
            _builder.Clear();
            return this;
        }

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

            if (_builder.Length > 0 && !_builder.ToString().EndsWith(';') && !_builder.ToString().EndsWith(' '))
                _builder.Append(' ');

            _builder.Append(property).Append(':').Append(value).Append(';');
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
            return condition && !string.IsNullOrWhiteSpace(value) 
                ? Append(property, value) 
                : this;
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
            return AppendIf(property, value, condition());
        }

        /// <summary>
        /// Conditionally appends a CSS property with its value if the condition is true.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendIf<TValue>(string property, TValue value, bool condition)
        {
            return condition ? Append(property, value?.ToString() ?? string.Empty) : this;
        }

        /// <summary>
        /// Conditionally appends a CSS property with its value if the condition function returns true.
        /// </summary>
        /// <param name="property">The CSS property name.</param>
        /// <param name="value">The CSS property value.</param>
        /// <param name="condition">The condition function to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendIf<TValue>(string property, TValue value, Func<bool> condition)
        {
            return AppendIf(property, value, condition());
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

            if (_builder.Length > 0 && !_builder.ToString().EndsWith(" ") && !style.StartsWith(" "))
                _builder.Append(" ");

            _builder.Append(style.Trim());
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
            var styleString = style.ToStyle()?.Build() ?? string.Empty;
            if (_builder.Length > 0 && !_builder.ToString().EndsWith(" ") && !styleString.StartsWith(" "))
                _builder.Append(" ");

            _builder.Append(styleString.Trim());
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
            return condition ? AppendStyle(style) : this;
        }

        /// <summary>
        /// Conditionally appends a raw CSS style string if the condition function returns true.
        /// </summary>
        /// <param name="style">The CSS style string to append.</param>
        /// <param name="condition">The condition function to evaluate.</param>
        /// <returns>The current StyleBuilder instance for method chaining.</returns>
        public StyleBuilder AppendStyleIf(string? style, Func<bool> condition)
        {
            return condition() ? AppendStyle(style) : this;
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

            _builder.Append(other._builder);

            return this;
        }

        /// <summary>
        /// Builds the final CSS style string.
        /// </summary>
        /// <returns>The constructed CSS style string.</returns>
        public string Build()
        {
            return _builder.ToString().Trim();
        }

        /// <summary>
        /// Implicitly converts a StyleBuilder to its string representation.
        /// </summary>
        /// <param name="builder">The StyleBuilder to convert.</param>
        public static implicit operator string(StyleBuilder builder)
        {
            return builder?.Build() ?? string.Empty;
        }
    }
}
