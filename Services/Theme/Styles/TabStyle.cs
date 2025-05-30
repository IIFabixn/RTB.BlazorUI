using RTB.BlazorUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.BlazorUI.Services.Theme.Styles
{
    public class TabStyle : RTBStyle
    {
        public string? Color { get; set; }

        public override string ToStyle()
        {
            return StyleBuilder.Create(base.ToStyle())
                .AppendIfNotEmpty("color", Color)
                .Build();
        }
    }
}
