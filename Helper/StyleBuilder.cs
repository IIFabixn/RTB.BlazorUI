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
        private StyleBuilder(string initStyle = "")
        {
            _buidler = new(initStyle);
        }

        public static StyleBuilder Create(string initStyle = "")
        {
            return new StyleBuilder(initStyle);
        }

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
