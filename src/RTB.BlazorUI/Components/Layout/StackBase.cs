using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Helper;
using static RTB.Blazor.Styled.Components.Flex;

namespace RTB.Blazor.Components.Layout
{
    /// <summary>
    /// Base class for layout stack components.
    /// A stack arranges its child content along a main axis (row or column),
    /// optionally reversing order, applying gaps, wrapping, and alignment similar to CSS flexbox.
    /// </summary>
    /// <remarks>
    /// Concrete implementations should define the default direction by overriding <see cref="GetDirection"/>.
    /// All parameters are optional. When null (or default false for booleans), the corresponding CSS is omitted,
    /// allowing styles to inherit or be controlled by parent components or CSS.
    /// </remarks>
    public abstract class StackBase : RTBComponent
    {
        /// <summary>
        /// The content to be rendered inside the stack.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;

        /// <summary>
        /// When true, reverses the main axis order (e.g., row-reverse or column-reverse).
        /// </summary>
        /// <remarks>
        /// Combines with <see cref="GetDirection"/> to determine the effective axis.
        /// </remarks>
        [Parameter] public bool Reverse { get; set; }

        /// <summary>
        /// The spacing (gap) between immediate children.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>gap</c>. When null, no explicit gap is emitted.
        /// </remarks>
        [Parameter] public Spacing? Gap { get; set; }

        /// <summary>
        /// Controls wrapping of child items along the main axis.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>flex-wrap</c>: NoWrap, Wrap, or WrapReverse.
        /// When null, no explicit wrap rule is emitted.
        /// </remarks>
        [Parameter] public WrapMode? Wrap { get; set; }

        /// <summary>
        /// Alignment of items on the cross axis.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>align-items</c>. When null, no explicit alignment is emitted.
        /// </remarks>
        [Parameter] public Align? AlignItem { get; set; }

        /// <summary>
        /// Distribution of items along the main axis.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>justify-content</c>. When null, no explicit justification is emitted.
        /// </remarks>
        [Parameter] public Justify? JustifyContent { get; set; }

        /// <summary>
        /// Shrink factor when this stack acts as a flex item within a parent flex container.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>flex-shrink</c>. When null, no explicit shrink is emitted.
        /// </remarks>
        [Parameter] public int? Shrink { get; set; }

        /// <summary>
        /// Grow factor when this stack acts as a flex item within a parent flex container.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>flex-grow</c>. When null, no explicit grow is emitted.
        /// </remarks>
        [Parameter] public int? Grow { get; set; }

        /// <summary>
        /// Background color applied to the stack container.
        /// </summary>
        /// <remarks>
        /// Maps to CSS <c>background-color</c>. When null, no explicit background is emitted.
        /// </remarks>
        [Parameter] public RTBColor? Background { get; set; }

        /// <summary>
        /// Inner spacing of the stack container.
        /// </summary>
        /// <remarks>
        /// Accepts 1 to 4 values using CSS padding shorthand semantics:
        /// 1 value: all sides; 2 values: vertical | horizontal; 3 values: top | horizontal | bottom; 4 values: top | right | bottom | left.
        /// When null or empty, no explicit padding is emitted.
        /// </remarks>
        [Parameter] public Spacing[]? Padding { get; set; }

        /// <summary>
        /// Outer spacing around the stack container.
        /// </summary>
        /// <remarks>
        /// Accepts 1 to 4 values using CSS margin shorthand semantics:
        /// 1 value: all sides; 2 values: vertical | horizontal; 3 values: top | horizontal | bottom; 4 values: top | right | bottom | left.
        /// When null or empty, no explicit margin is emitted.
        /// </remarks>
        [Parameter] public Spacing[]? Margin { get; set; }

        /// <summary>
        /// Determine the base axis direction of this stack.
        /// </summary>
        /// <returns>
        /// The intended axis direction (<see cref="AxisDirection.Row"/> or <see cref="AxisDirection.Column"/>),
        /// optionally including reverse variants. A null value allows defaults to be applied by derived components.
        /// </returns>
        /// <remarks>
        /// Implementations may combine the returned direction with the <see cref="Reverse"/> flag
        /// to compute the effective main axis.
        /// </remarks>
        protected abstract AxisDirection? GetDirection();
    }
}
