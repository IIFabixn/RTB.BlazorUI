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
        public static StyleBuilder Create(string initStyle = "")
        {
            return new StyleBuilder(initStyle);
        }

        /// <summary>
        /// Will append a style to the builder.
        /// If the style or value is null or whitespace, it will not append anything.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public StyleBuilder Append(string style, string? value)
        {
            if (!string.IsNullOrWhiteSpace(style) && !string.IsNullOrWhiteSpace(value))
            {
                _buidler.Append($"{style}: {value};");
            }

            return this;
        }

        public StyleBuilder AppendIf(string style, string value, Func<bool> condition)
        {
            return condition() ? Append(style, value) : this;
        }

        public StyleBuilder AppendIf<TValue>(string style, TValue value, Func<bool> condition)
        {
            return condition() ? Append(style, value?.ToString() ?? string.Empty) : this;
        }

        public StyleBuilder AppendIf(string style, string value, bool condition)
        {
            return condition ? Append(style, value) : this;
        }

        public StyleBuilder AppendIf<TValue>(string style, TValue value, bool condition)
        {
            return condition ? Append(style, value?.ToString() ?? string.Empty) : this;
        }

        public string Build()
        {
            return _buidler.ToString().Trim();
        }

        public override string ToString()
        {
            return Build();
        }
    }
}
