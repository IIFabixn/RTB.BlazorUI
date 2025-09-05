using Microsoft.AspNetCore.Components;
using RTB.Blazor.Styled.Core;
using RTB.Blazor.Styled.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RTB.Blazor.Styled.Components.Flex;

namespace RTB.Blazor.Styled.Components
{
    /// <summary>
    /// Contributes typographic (text-related) CSS declarations to a cascading <see cref="StyleBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code>
    /// &lt;StyleRoot&gt;
    ///   &lt;Typography FontSize="14" FontWeight="600" Color="@(RTBColor.FromRgb(30,30,30))" /&gt;
    /// &lt;/StyleRoot&gt;
    /// </code>
    /// Each non-null parameter is translated to its corresponding CSS property during style composition.
    /// Parameters left <c>null</c> produce no output (zero cost).
    /// Enumeration values are converted via internal <c>ToCss()</c> helpers (not shown here).
    /// </remarks>
    public class Typography : RTBStyleBase
    {
        /// <summary>
        /// Horizontal alignment of inline content (maps to CSS <c>text-align</c>).
        /// </summary>
        public enum TextAlign
        {
            /// <summary>Align left (default browser behavior).</summary>
            Left,
            /// <summary>Align right.</summary>
            Right,
            /// <summary>Center align.</summary>
            Center,
            /// <summary>Distribute text evenly (maps to <c>justify</c>).</summary>
            Justify
        }

        /// <summary>
        /// Decoration style applied to under/over/line-through (CSS <c>text-decoration-style</c>).
        /// </summary>
        public enum TextDecorationStyle
        {
            /// <summary>Solid line.</summary>
            Solid,
            /// <summary>Double line.</summary>
            Double,
            /// <summary>Dotted line.</summary>
            Dotted,
            /// <summary>Dashed line.</summary>
            Dashed,
            /// <summary>Wavy line.</summary>
            Wavy
        }

        /// <summary>
        /// Decoration line types (CSS <c>text-decoration-line</c>).
        /// Multiple values can be combined (e.g., underline + overline).
        /// </summary>
        public enum TextDecorationLine
        {
            /// <summary>No decoration (suppresses others).</summary>
            None,
            /// <summary>Underline baseline.</summary>
            Underline,
            /// <summary>Overline above text.</summary>
            Overline,
            /// <summary>Line through (strike).</summary>
            LineThrough,
            /// <summary>Historical / non-standard (<c>blink</c> – generally ignored by modern browsers).</summary>
            Blink,
            /// <summary>Indicates spelling error styling (non-standard keyword).</summary>
            SpellError,
            /// <summary>Indicates grammar error styling (non-standard keyword).</summary>
            GrammarError
        }

        /// <summary>
        /// Text foreground color (CSS <c>color</c>). Omitted when <c>null</c>.
        /// </summary>
        [Parameter] public RTBColor? Color { get; set; }

        /// <summary>
        /// Font size (CSS <c>font-size</c>). Provide a valid CSS length or keyword (e.g., "14px", "1rem", "small").
        /// </summary>
        [Parameter] public string? FontSize { get; set; }

        /// <summary>
        /// Font weight (CSS <c>font-weight</c>): numeric ("400", "600") or keywords ("bold", "lighter").
        /// </summary>
        [Parameter] public string? FontWeight { get; set; }

        /// <summary>
        /// Line height (CSS <c>line-height</c>): unit, number, or keyword ("normal").
        /// </summary>
        [Parameter] public string? LineHeight { get; set; }

        /// <summary>
        /// Color for text decorations (CSS <c>text-decoration-color</c>).
        /// </summary>
        [Parameter] public RTBColor? DecorationColor { get; set; }

        /// <summary>
        /// Decoration stroke style (CSS <c>text-decoration-style</c>).
        /// </summary>
        [Parameter] public TextDecorationStyle? DecorationStyle { get; set; }

        /// <summary>
        /// One or more decoration line types (CSS <c>text-decoration-line</c>). Ignored when null or empty.
        /// </summary>
        [Parameter] public TextDecorationLine[]? DecorationLine { get; set; }

        /// <summary>
        /// Decoration stroke thickness (CSS <c>text-decoration-thickness</c>). Uses <see cref="SizeUnit.ToString"/>.
        /// </summary>
        [Parameter] public SizeUnit? DecorationThickness { get; set; }

        /// <summary>
        /// Horizontal text alignment (CSS <c>text-align</c>).
        /// </summary>
        [Parameter] public TextAlign? Align { get; set; }

        /// <summary>
        /// Font family stack (CSS <c>font-family</c>). Provide raw CSS (e.g., "Inter, system-ui, sans-serif").
        /// </summary>
        [Parameter] public string? FontFamily { get; set; }

        /// <summary>
        /// Additional spacing between characters (CSS <c>letter-spacing</c>), e.g., "0.05em".
        /// </summary>
        [Parameter] public string? LetterSpacing { get; set; }

        /// <summary>
        /// Text transform (CSS <c>text-transform</c>): e.g., "uppercase", "capitalize".
        /// </summary>
        [Parameter] public string? TextTransform { get; set; }

        /// <summary>
        /// Whitespace processing (CSS <c>white-space</c>): e.g., "nowrap", "pre", "pre-wrap".
        /// </summary>
        [Parameter] public string? WhiteSpace { get; set; }

        /// <summary>
        /// Word breaking rules (CSS <c>word-break</c>): e.g., "break-word", "keep-all".
        /// </summary>
        [Parameter] public string? WordBreak { get; set; }

        /// <summary>
        /// Soft wrap opportunity behavior (CSS <c>overflow-wrap</c> / legacy <c>word-wrap</c>): e.g., "anywhere".
        /// </summary>
        [Parameter] public string? OverflowWrap { get; set; }

        /// <summary>
        /// Text overflow handling (CSS <c>text-overflow</c>): e.g., "ellipsis", "clip".
        /// </summary>
        [Parameter] public string? TextOverflow { get; set; }

        /// <summary>
        /// Populates the provided <paramref name="builder"/> with text-related declarations.
        /// Only emits properties when their corresponding parameters are non-null (or non-empty for arrays).
        /// Enumeration values are converted via extension helpers (<c>ToCss()</c>).
        /// </summary>
        /// <param name="builder">Target style builder (never null).</param>
        protected override void BuildStyle(StyleBuilder builder)
        {
            builder
                .Color(Color)
                .SetIfNotNull("font-size", FontSize)
                .SetIfNotNull("font-weight", FontWeight)
                .SetIfNotNull("line-height", LineHeight)
                .SetIfNotNull("text-decoration-color", DecorationColor)
                .SetIfNotNull("text-decoration-style", DecorationStyle?.ToCss())
                .SetIf("text-decoration-line", string.Join(" ", DecorationLine!.Select(dl => dl.ToCss())), DecorationLine is not null and { Length: > 0 })
                .SetIfNotNull("text-decoration-thickness", DecorationThickness)
                .SetIfNotNull("text-overflow", TextOverflow)
                .SetIfNotNull("text-align", Align?.ToCss())
                .SetIfNotNull("font-family", FontFamily)
                .SetIfNotNull("letter-spacing", LetterSpacing)
                .SetIfNotNull("text-transform", TextTransform)
                .SetIfNotNull("white-space", WhiteSpace)
                .SetIfNotNull("word-break", WordBreak)
                .SetIfNotNull("overflow-wrap", OverflowWrap);
        }
    }
}
