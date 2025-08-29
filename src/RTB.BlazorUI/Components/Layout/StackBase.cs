using Microsoft.AspNetCore.Components;
using RTB.Blazor.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RTB.Blazor.Styled.Components.Flex;

namespace RTB.Blazor.UI.Components.Layout
{
    public abstract class StackBase : RTBComponent
    {
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        [Parameter] public bool Reverse { get; set; }

        [Parameter] public Spacing? Gap { get; set; }
        [Parameter] public WrapMode? Wrap { get; set; }
        [Parameter] public Align? AlignItem { get; set; }
        [Parameter] public Justify? JustifyContent { get; set; }

        [Parameter] public int? Shrink { get; set; }
        [Parameter] public int? Grow { get; set; }

        [Parameter] public RTBColor? Background { get; set; }
        [Parameter] public Spacing[]? Padding { get; set; }
        [Parameter] public Spacing[]? Margin { get; set; }

        protected abstract AxisDirection? GetDirection();
    }
}
