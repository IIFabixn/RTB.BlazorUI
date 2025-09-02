using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper
{
    /// <summary>
    /// Base type for a small, composable CSS selector DSL.
    /// Use provided factory helpers, static members, and operator overloads to build complex selectors
    /// in a type-safe way and render them to CSS with <see cref="ToString"/>.
    /// </summary>
    /// <example>
    /// var s =
    ///     SelectorExpression.Element("ul")
    ///     / SelectorExpression.Element("li")
    ///     + SelectorExpression.PseudoClass("hover")
    ///     > SelectorExpression.Element("a") &amp; SelectorExpression.Class("primary");
    /// // Renders: "ul li+ :hover > a~.primary"
    /// var css = s.ToString();
    /// </example>
    public abstract record SelectorExpression
    {
        // Factory Helpers

        /// <summary>
        /// Creates a selector expression for an HTML element by its name.
        /// </summary>
        /// <param name="name">HTML tag name (e.g., "div", "button").</param>
        public static SelectorExpression Element(string name) => new ElementSelector(name);

        /// <summary>
        /// Creates a selector expression for an element with a specific ID.
        /// </summary>
        /// <param name="id">ID without the leading '#'.</param>
        public static SelectorExpression Id(string id) => new IdSelector(id);

        /// <summary>
        /// Creates a selector expression for an element with a specific class name.
        /// </summary>
        /// <param name="cls">Class name without the leading '.'.</param>
        public static SelectorExpression Class(string cls) => new ClassSelector(cls);

        /// <summary>
        /// Creates a selector expression for an attribute of an element.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Optional attribute value. If omitted, renders as [name].</param>
        /// <param name="op">
        /// Optional operator (e.g., "=", "~=", "^=", "$=", "*=", "|="). Defaults to "=" when <paramref name="value"/> is provided.
        /// </param>
        public static SelectorExpression Attribute(string name, string? value = null, string? op = null) => new AttributeSelector(name, value, op);

        /// <summary>
        /// Creates a selector expression for a CSS pseudo-class.
        /// </summary>
        /// <param name="name">Pseudo-class name without the leading ':'.</param>
        public static SelectorExpression PseudoClass(string name) => new PseudoClass(name);

        /// <summary>
        /// Creates a selector expression for a CSS pseudo-element.
        /// </summary>
        /// <param name="name">Pseudo-element name without the leading '::'.</param>
        public static SelectorExpression PseudoElement(string name) => new PseudoElement(name);

        /// <summary>
        /// Implicitly converts a raw string to a <see cref="SelectorExpression"/>. The string is used as-is.
        /// </summary>
        /// <param name="literal">Raw CSS selector text.</param>
        public static implicit operator SelectorExpression(string literal) => new RawSelector(literal);

        /// <summary>
        /// Implicitly converts a <see cref="SelectorExpression"/> to its string (CSS) representation.
        /// </summary>
        /// <param name="expr">Selector expression instance.</param>
        public static implicit operator string?(SelectorExpression? expr) => expr?.ToString();

        /// <summary>
        /// Renders the selector to CSS.
        /// </summary>
        public override string ToString() => Render();

        /// <summary>
        /// Renders the selector to CSS. Implemented by derived records.
        /// </summary>
        internal protected abstract string Render();

        // Operators for combinators

        /// <summary>
        /// Child combinator: selects direct children.
        /// </summary>
        /// <param name="parent">Left selector (parent).</param>
        /// <param name="child">Right selector (direct child).</param>
        /// <returns>A combined selector using the '&gt;' combinator.</returns>
        /// <example>
        /// SelectorExpression.Element("ul") &gt; SelectorExpression.Element("li") // "ul&gt;li"
        /// </example>
        public static SelectorExpression operator >(SelectorExpression parent, SelectorExpression child)
            => new BinarySelector(parent, ">", child);

        /// <summary>
        /// Not supported in CSS selector combinators.
        /// </summary>
        [Obsolete("The '<' operator is not valid for CSS selectors.", error: true)]
        public static SelectorExpression operator <(SelectorExpression parent, SelectorExpression child) => throw new NotSupportedException("The '<' operator is not supported for CSS selectors.");

        /// <summary>
        /// Descendant combinator: selects any level descendant (whitespace).
        /// </summary>
        /// <param name="parent">Ancestor selector.</param>
        /// <param name="descendant">Descendant selector.</param>
        /// <returns>A combined selector using a space.</returns>
        /// <example>
        /// SelectorExpression.Element("ul") / SelectorExpression.Element("li") // "ul li"
        /// </example>
        public static SelectorExpression operator /(SelectorExpression parent, SelectorExpression descendant)
            => new BinarySelector(parent, " ", descendant);

        /// <summary>
        /// Adjacent sibling combinator: selects the immediate following sibling.
        /// </summary>
        /// <example>
        /// SelectorExpression.Class("item") + SelectorExpression.Class("badge") // ".item+.badge"
        /// </example>
        public static SelectorExpression operator +(SelectorExpression a, SelectorExpression b)
            => new BinarySelector(a, "+", b);

        /// <summary>
        /// General sibling combinator: selects any following sibling.
        /// </summary>
        /// <example>
        /// SelectorExpression.Class("item") &amp; SelectorExpression.Class("badge") // ".item~.badge"
        /// </example>
        public static SelectorExpression operator &(SelectorExpression a, SelectorExpression b)
            => new BinarySelector(a, "~", b);

        /// <summary>
        /// Grouping selector: combines selectors separated by commas.
        /// </summary>
        /// <example>
        /// (SelectorExpression.Element("h1") | SelectorExpression.Element("h2")).ToString() // "h1, h2"
        /// </example>
        public static SelectorExpression operator |(SelectorExpression a, SelectorExpression b)
            => new GroupedSelector([a, b]);
    }

    /// <summary>
    /// CSS selector for an HTML element by name.
    /// </summary>
    /// <param name="Name">HTML tag name.</param>
    public sealed record ElementSelector(string Name) : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render() => Name;
    }

    /// <summary>
    /// CSS selector for an element with a specific ID.
    /// </summary>
    /// <param name="IdName">ID without the leading '#'.</param>
    public sealed record IdSelector(string IdName) : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render() => $"#{IdName}";
    }

    /// <summary>
    /// CSS selector for an element with a specific class name.
    /// </summary>
    /// <param name="ClassName">Class name without the leading '.'.</param>
    public sealed record ClassSelector(string ClassName) : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render() => $".{ClassName}";
    }

    /// <summary>
    /// CSS attribute selector.
    /// </summary>
    /// <param name="Name">Attribute name.</param>
    /// <param name="Value">Optional attribute value; if null renders as [name].</param>
    /// <param name="Op">Optional operator (defaults to "=" when <paramref name="Value"/> is provided).</param>
    /// <remarks>
    /// Supported operators include '=', '~=', '^=', '$=', '*=', and '|='.
    /// </remarks>
    public sealed record AttributeSelector(string Name, string? Value = null, string? Op = null) : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render()
        {
            if (Value is null) return $"[{Name}]";
            return $"[{Name}{Op ?? "="}\"{Value}\"]";
        }
    }

    /// <summary>
    /// CSS pseudo-class selector (non-functional).
    /// </summary>
    /// <param name="Name">Pseudo-class name without the leading ':'.</param>
    public sealed record PseudoClass(string Name) : SelectorExpression
    {
        /* -------------------------
         * Structural pseudo-classes
         * ------------------------- */
        /// <summary>:root</summary>
        public static PseudoClass Root => new("root");
        /// <summary>:empty</summary>
        public static PseudoClass Empty => new("empty");
        /// <summary>:first-child</summary>
        public static PseudoClass FirstChild => new("first-child");
        /// <summary>:last-child</summary>
        public static PseudoClass LastChild => new("last-child");
        /// <summary>:only-child</summary>
        public static PseudoClass OnlyChild => new("only-child");
        /// <summary>:first-of-type</summary>
        public static PseudoClass FirstOfType => new("first-of-type");
        /// <summary>:last-of-type</summary>
        public static PseudoClass LastOfType => new("last-of-type");
        /// <summary>:only-of-type</summary>
        public static PseudoClass OnlyOfType => new("only-of-type");

        /* -------------------------
         * User action / interaction
         * ------------------------- */
        /// <summary>:hover</summary>
        public static PseudoClass Hover => new("hover");
        /// <summary>:active</summary>
        public static PseudoClass Active => new("active");
        /// <summary>:focus</summary>
        public static PseudoClass Focus => new("focus");
        /// <summary>:focus-within</summary>
        public static PseudoClass FocusWithin => new("focus-within");
        /// <summary>:focus-visible</summary>
        public static PseudoClass FocusVisible => new("focus-visible");

        /* -------------------------
         * Link / navigation
         * ------------------------- */
        /// <summary>:any-link</summary>
        public static PseudoClass AnyLink => new("any-link");
        /// <summary>:link</summary>
        public static PseudoClass Link => new("link");
        /// <summary>:visited</summary>
        public static PseudoClass Visited => new("visited");
        /// <summary>:local-link</summary>
        public static PseudoClass LocalLink => new("local-link");
        /// <summary>:target</summary>
        public static PseudoClass Target => new("target");
        /// <summary>:target-within</summary>
        public static PseudoClass TargetWithin => new("target-within");
        /// <summary>:scope</summary>
        public static PseudoClass Scope => new("scope");

        /* -------------------------
         * Form & input states
         * ------------------------- */
        /// <summary>:enabled</summary>
        public static PseudoClass Enabled => new("enabled");
        /// <summary>:disabled</summary>
        public static PseudoClass Disabled => new("disabled");
        /// <summary>:read-only</summary>
        public static PseudoClass ReadOnly => new("read-only");
        /// <summary>:read-write</summary>
        public static PseudoClass ReadWrite => new("read-write");
        /// <summary>:placeholder-shown</summary>
        public static PseudoClass PlaceholderShown => new("placeholder-shown");
        /// <summary>:autofill</summary>
        public static PseudoClass Autofill => new("autofill");
        /// <summary>:default</summary>
        public static PseudoClass Default => new("default");
        /// <summary>:checked</summary>
        public static PseudoClass Checked => new("checked");
        /// <summary>:indeterminate</summary>
        public static PseudoClass Indeterminate => new("indeterminate");
        /// <summary>:blank</summary>
        public static PseudoClass Blank => new("blank");
        /// <summary>:valid</summary>
        public static PseudoClass Valid => new("valid");
        /// <summary>:invalid</summary>
        public static PseudoClass Invalid => new("invalid");
        /// <summary>:in-range</summary>
        public static PseudoClass InRange => new("in-range");
        /// <summary>:out-of-range</summary>
        public static PseudoClass OutOfRange => new("out-of-range");
        /// <summary>:required</summary>
        public static PseudoClass Required => new("required");
        /// <summary>:optional</summary>
        public static PseudoClass Optional => new("optional");
        /// <summary>:user-valid</summary>
        public static PseudoClass UserValid => new("user-valid");
        /// <summary>:user-invalid</summary>
        public static PseudoClass UserInvalid => new("user-invalid");

        /* -------------------------
         * Resource / media state
         * ------------------------- */
        /// <summary>:playing</summary>
        public static PseudoClass Playing => new("playing");
        /// <summary>:paused</summary>
        public static PseudoClass Paused => new("paused");
        /// <summary>:seeking</summary>
        public static PseudoClass Seeking => new("seeking");
        /// <summary>:buffering</summary>
        public static PseudoClass Buffering => new("buffering");
        /// <summary>:stalled</summary>
        public static PseudoClass Stalled => new("stalled");
        /// <summary>:muted</summary>
        public static PseudoClass Muted => new("muted");
        /// <summary>:volume-locked</summary>
        public static PseudoClass VolumeLocked => new("volume-locked");

        /* -------------------------
         * Temporal
         * ------------------------- */
        /// <summary>:current</summary>
        public static PseudoClass Current => new("current");
        /// <summary>:past</summary>
        public static PseudoClass Past => new("past");
        /// <summary>:future</summary>
        public static PseudoClass Future => new("future");

        /* -------------------------
         * Element & display state
         * ------------------------- */
        /// <summary>:defined</summary>
        public static PseudoClass Defined => new("defined");
        /// <summary>:open</summary>
        public static PseudoClass Open => new("open");
        /// <summary>:modal</summary>
        public static PseudoClass Modal => new("modal");
        /// <summary>:fullscreen</summary>
        public static PseudoClass Fullscreen => new("fullscreen");
        /// <summary>:popover-open</summary>
        public static PseudoClass PopoverOpen => new("popover-open");
        /// <summary>:picture-in-picture</summary>
        public static PseudoClass PictureInPicture => new("picture-in-picture");

        /* -------------------------
         * Shadow DOM / components
         * ------------------------- */
        /// <summary>:host</summary>
        public static PseudoClass Host => new("host");

        /// <inheritdoc />
        internal protected override string Render() => $":{Name}";
    }

    /// <summary>
    /// CSS functional pseudo-class selector, which can take arguments.
    /// </summary>
    /// <param name="Name">Pseudo-class name without the leading ':'.</param>
    /// <param name="Arguments">Arguments to the functional pseudo-class.</param>
    public sealed record FunctionalPseudoClass(string Name, IEnumerable<SelectorExpression> Arguments) : SelectorExpression
    {
        /* -------------------------
         * Functional pseudo-classes
         * ------------------------- */
        /// <summary>:has(...)</summary>
        public static FunctionalPseudoClass Has(params SelectorExpression[] args) => new("has", args);
        /// <summary>:not(...)</summary>
        public static FunctionalPseudoClass Not(params SelectorExpression[] args) => new("not", args);
        /// <summary>:is(...)</summary>
        public static FunctionalPseudoClass Is(params SelectorExpression[] args) => new("is", args);
        /// <summary>:where(...)</summary>
        public static FunctionalPseudoClass Where(params SelectorExpression[] args) => new("where", args);
        /// <summary>:lang(code)</summary>
        public static FunctionalPseudoClass Lang(string code) => new("lang", [new RawSelector(code)]);
        /// <summary>:dir(direction)</summary>
        public static FunctionalPseudoClass Dir(string direction) => new("dir", [new RawSelector(direction)]);

        /// <summary>:nth-child(expr)</summary>
        public static FunctionalPseudoClass NthChild(string expr) => new("nth-child", [new RawSelector(expr)]);
        /// <summary>:nth-last-child(expr)</summary>
        public static FunctionalPseudoClass NthLastChild(string expr) => new("nth-last-child", [new RawSelector(expr)]);
        /// <summary>:nth-of-type(expr)</summary>
        public static FunctionalPseudoClass NthOfType(string expr) => new("nth-of-type", [new RawSelector(expr)]);
        /// <summary>:nth-last-of-type(expr)</summary>
        public static FunctionalPseudoClass NthLastOfType(string expr) => new("nth-last-of-type", [new RawSelector(expr)]);

        /* -------------------------
         * Shadow DOM / components
         * ------------------------- */
        /// <summary>:host(...)</summary>
        public static FunctionalPseudoClass HostSelector(params SelectorExpression[] args) => new("host", args);
        /// <summary>:host-context(...)</summary>
        public static FunctionalPseudoClass HostContext(params SelectorExpression[] args) => new("host-context", args);
        /// <summary>(Proposed) :has-slotted(...)</summary>
        public static FunctionalPseudoClass HasSlotted(params SelectorExpression[] args) => new("has-slotted", args);

        /// <inheritdoc />
        internal protected override string Render()
            => $":{Name}({string.Join(", ", Arguments.Select(a => a.Render()))})";
    }

    /// <summary>
    /// Parent selector reference used in nested rules to refer to the current selector,
    /// similar to '&amp;' in SCSS. Useful for composing states or BEM-style suffixes.
    /// </summary>
    public sealed record ParentSelector() : SelectorExpression
    {
        /// <summary>
        /// Gets a new instance representing the current selector "&amp;".
        /// </summary>
        public static ParentSelector Parent => new();
        /// <inheritdoc />
        internal protected override string Render() => "&";
    }

    /// <summary>
    /// CSS pseudo-element selector.
    /// </summary>
    /// <param name="Name">Pseudo-element name without the leading '::'.</param>
    public sealed record PseudoElement(string Name) : SelectorExpression
    {
        /* -------------------------
         * Pseudo-elements
         * ------------------------- */
        /// <summary>::before</summary>
        public static PseudoElement Before => new("before");
        /// <summary>::after</summary>
        public static PseudoElement After => new("after");
        /// <summary>::first-line</summary>
        public static PseudoElement FirstLine => new("first-line");
        /// <summary>::first-letter</summary>
        public static PseudoElement FirstLetter => new("first-letter");
        /// <summary>::selection</summary>
        public static PseudoElement Selection => new("selection");
        /// <summary>::marker</summary>
        public static PseudoElement Marker => new("marker");
        /// <summary>::placeholder</summary>
        public static PseudoElement Placeholder => new("placeholder");
        /// <summary>::file-selector-button</summary>
        public static PseudoElement FileSelectorButton => new("file-selector-button");
        /// <summary>::backdrop</summary>
        public static PseudoElement Backdrop => new("backdrop");
        /// <summary>::cue</summary>
        public static PseudoElement Cue => new("cue");
        /// <summary>::slotted(name)</summary>
        /// <param name="name">The slotted selector argument.</param>
        public static PseudoElement Slotted(string name) => new($"slotted({name})");

        /// <inheritdoc />
        internal protected override string Render() => $"::{Name}";
    }

    /// <summary>
    /// Raw CSS selector expression. The content is emitted without transformation.
    /// </summary>
    /// <param name="Raw">Raw CSS selector text.</param>
    public sealed record RawSelector(string Raw) : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render() => Raw;
    }

    /// <summary>
    /// Binary selector expression combining two selectors with a combinator/operator.
    /// </summary>
    /// <param name="Left">Left-hand selector.</param>
    /// <param name="Operator">The operator (e.g., " ", "&gt;", "+", "~").</param>
    /// <param name="Right">Right-hand selector.</param>
    public sealed record BinarySelector(SelectorExpression Left, string Operator, SelectorExpression Right)
        : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render()
        {
            if (Operator == " ")
                return $"{Left.Render()} {Right.Render()}";
            return $"{Left.Render()}{Operator}{Right.Render()}";
        }
    }

    /// <summary>
    /// Group of selectors separated by commas.
    /// </summary>
    /// <param name="Selectors">Selectors to group.</param>
    public sealed record GroupedSelector(IEnumerable<SelectorExpression> Selectors) : SelectorExpression
    {
        /// <inheritdoc />
        internal protected override string Render() => string.Join(", ", Selectors.Select(s => s.Render()));
    }
}
