using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Styles
{
    /// <summary>
    /// Action abstractions for deferred execution
    /// </summary>
    internal abstract class StyleAction
    {
        public virtual void Apply(StringBuilder builder)
        {
            if (builder.Length > 0)
            {
                char lastChar = builder[^1];
                if (lastChar != ';' && lastChar != ' ')
                    builder.Append(' ');
            }
        }
    }

    internal class PropertyAction(string property, string value) : StyleAction
    {
        private readonly string _property = property.Trim();
        private readonly string _value = value.Trim();

        public override void Apply(StringBuilder builder)
        {
            base.Apply(builder);

            builder.Append(_property).Append(':').Append(_value).Append(';');
        }
    }

    internal class RawStyleAction(string style) : StyleAction
    {
        private readonly string _style = style.Trim();

        public override void Apply(StringBuilder builder)
        {
            base.Apply(builder);

            builder.Append(_style);
        }
    }

    internal class StyleObjectAction(IStyle style) : StyleAction
    {
        private readonly IStyle _style = style;

        public override void Apply(StringBuilder builder)
        {
            var styleString = _style.ToStyle()?.Build()?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(styleString)) return;

            base.Apply(builder);

            builder.Append(styleString);
        }
    }

    internal class SelectorAction(string selector, string style) : StyleAction
    {
        private readonly string _selector = selector;
        private readonly string _style = style;

        public override void Apply(StringBuilder builder)
        {
            base.Apply(builder);
            builder.Append($"{_selector} {{ ");
            builder.Append(_style);
            builder.Append(" }");
        }
    }
}
