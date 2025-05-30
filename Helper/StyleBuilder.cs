using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Helper
{
    public class StyleBuilder
    {
        private readonly StringBuilder _buidler;
        protected StyleBuilder(string initStyle = "")
        {
            _buidler = new(initStyle);
        }

        /// <summary>
        /// Creates a new instance of StyleBuilder with an optional initial style.
        /// </summary>
        /// <param name="initStyle">Initial style string to start with.</param>
        /// <returns>A new instance of StyleBuilder.</returns>
        public static StyleBuilder Create(params string?[]? initStyles)
        {
            if (initStyles is null || initStyles.Length == 0) return new StyleBuilder();
            return new StyleBuilder(string.Join("; ", initStyles.Where(s => !string.IsNullOrEmpty(s))));
        }

        /// <summary>
        /// Will append a style to the builder.
        /// If the style or value is null or whitespace, it will not append anything.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public StyleBuilder Append(string style, string value)
        {
            _buidler.Append($"{style}: {value};");

            return this;
        }

        public StyleBuilder AppendIf(string style, string? value, Func<bool> condition)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            return condition() ? Append(style, value) : this;
        }

        public StyleBuilder AppendIfNotEmpty(string style, string? value)
        {
            return AppendIf(style, value, !string.IsNullOrEmpty(value));
        }

        public StyleBuilder AppendIf<TValue>(string style, TValue value, Func<bool> condition)
        {
            return condition() ? Append(style, value?.ToString() ?? string.Empty) : this;
        }

        public StyleBuilder AppendIf(string style, string? value, bool condition)
        {
            if (string.IsNullOrEmpty(value)) return this;

            return condition ? Append(style, value) : this;
        }

        public StyleBuilder AppendIf<TValue>(string style, TValue value, bool condition)
        {
            return condition ? Append(style, value?.ToString() ?? string.Empty) : this;
        }

        public StyleBuilder AppendStyle(string? style)
        {
            if (!string.IsNullOrWhiteSpace(style))
            {
                _buidler.Append(style);
            }

            return this;
        }

        public StyleBuilder AppendStyleIf(string? style, bool condition)
        {
            return condition ? AppendStyle(style) : this;
        }

        public StyleBuilder AppendStyleIf(string? style, Func<bool> condition)
        {
            return condition() ? AppendStyle(style) : this;
        }


        public string Build()
        {
            return _buidler.ToString().Trim();
        }
    }
}
