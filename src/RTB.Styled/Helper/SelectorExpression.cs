using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Helper
{
    public abstract record SelectorExpression
    {
        // Factory Helpers

        /// <summary>
        /// Creates a selector expression for an HTML element by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SelectorExpression Element(string name) => new ElementSelector(name);

        /// <summary>
        /// Creates a selector expression for an element with a specific ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SelectorExpression Id(string id) => new IdSelector(id);

        /// <summary>
        /// Creates a selector expression for an element with a specific class name.
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public static SelectorExpression Class(string cls) => new ClassSelector(cls);

        /// <summary>
        /// Creates a selector expression for an attribute of an element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static SelectorExpression Attribute(string name, string? value = null, string? op = null) => new AttributeSelector(name, value, op);

        /// <summary>
        /// Creates a selector expression for the parent of the current element.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SelectorExpression PseudoClass(string name) => new PseudoClass(name);

        /// <summary>
        /// Creates a selector expression for the parent of the current element.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SelectorExpression PseudoElement(string name) => new PseudoElement(name);

        // Implicit conversions
        public static implicit operator SelectorExpression(string literal) => new RawSelector(literal);
        public static implicit operator string?(SelectorExpression? expr) => expr?.ToString();

        public override string ToString() => Render();
        internal protected abstract string Render();

        // Operators for combinators

        /// <summary>
        /// Represents a child selector in CSS.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static SelectorExpression operator >(SelectorExpression parent, SelectorExpression child)
            => new BinarySelector(parent, ">", child);

        [Obsolete("The '<' operator is not valid for CSS selectors.", error: true)]
        public static SelectorExpression operator <(SelectorExpression parent, SelectorExpression child) => throw new NotSupportedException("The '<' operator is not supported for CSS selectors.");

        /// <summary>
        /// Represents a descendant selector in CSS.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="descendant"></param>
        /// <returns></returns>
        public static SelectorExpression operator /(SelectorExpression parent, SelectorExpression descendant)
            => new BinarySelector(parent, " ", descendant);

        /// <summary>
        /// Represents a sibling selector in CSS.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SelectorExpression operator +(SelectorExpression a, SelectorExpression b)
            => new BinarySelector(a, "+", b);

        /// <summary>
        /// Represents a general sibling selector in CSS.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SelectorExpression operator &(SelectorExpression a, SelectorExpression b)
            => new BinarySelector(a, "~", b);

        /// <summary>
        /// Represents a group of selectors in CSS.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SelectorExpression operator |(SelectorExpression a, SelectorExpression b)
            => new GroupedSelector([a, b]);
    }

    /// <summary>
    /// Represents a CSS selector for an HTML element.
    /// </summary>
    /// <param name="Name"></param>
    public sealed record ElementSelector(string Name) : SelectorExpression
    {
        internal protected override string Render() => Name;
    }

    /// <summary>
    /// Represents a CSS selector for an element with a specific ID.
    /// </summary>
    /// <param name="IdName"></param>
    public sealed record IdSelector(string IdName) : SelectorExpression
    {
        internal protected override string Render() => $"#{IdName}";
    }

    /// <summary>
    /// Represents a CSS selector for an element with a specific class name.
    /// </summary>
    /// <param name="ClassName"></param>
    public sealed record ClassSelector(string ClassName) : SelectorExpression
    {
        internal protected override string Render() => $".{ClassName}";
    }

    /// <summary>
    /// Represents a CSS attribute selector.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Value"></param>
    /// <param name="Op"></param>
    public sealed record AttributeSelector(string Name, string? Value = null, string? Op = null) : SelectorExpression
    {
        internal protected override string Render()
        {
            if (Value is null) return $"[{Name}]";
            return $"[{Name}{Op ?? "="}\"{Value}\"]";
        }
    }

    /// <summary>
    /// Represents a CSS pseudo-class selector.
    /// </summary>
    /// <param name="Name"></param>
    public sealed record PseudoClass(string Name) : SelectorExpression
    {
        /* -------------------------
         * Structural pseudo-classes
         * ------------------------- */
        public static PseudoClass Root => new("root");
        public static PseudoClass Empty => new("empty");
        public static PseudoClass FirstChild => new("first-child");
        public static PseudoClass LastChild => new("last-child");
        public static PseudoClass OnlyChild => new("only-child");
        public static PseudoClass FirstOfType => new("first-of-type");
        public static PseudoClass LastOfType => new("last-of-type");
        public static PseudoClass OnlyOfType => new("only-of-type");

        /* -------------------------
         * User action / interaction
         * ------------------------- */
        public static PseudoClass Hover => new("hover");
        public static PseudoClass Active => new("active");
        public static PseudoClass Focus => new("focus");
        public static PseudoClass FocusWithin => new("focus-within");
        public static PseudoClass FocusVisible => new("focus-visible");

        /* -------------------------
         * Link / navigation
         * ------------------------- */
        public static PseudoClass AnyLink => new("any-link");
        public static PseudoClass Link => new("link");
        public static PseudoClass Visited => new("visited");
        public static PseudoClass LocalLink => new("local-link");
        public static PseudoClass Target => new("target");
        public static PseudoClass TargetWithin => new("target-within");
        public static PseudoClass Scope => new("scope");

        /* -------------------------
         * Form & input states
         * ------------------------- */
        public static PseudoClass Enabled => new("enabled");
        public static PseudoClass Disabled => new("disabled");
        public static PseudoClass ReadOnly => new("read-only");
        public static PseudoClass ReadWrite => new("read-write");
        public static PseudoClass PlaceholderShown => new("placeholder-shown");
        public static PseudoClass Autofill => new("autofill");
        public static PseudoClass Default => new("default");
        public static PseudoClass Checked => new("checked");
        public static PseudoClass Indeterminate => new("indeterminate");
        public static PseudoClass Blank => new("blank");
        public static PseudoClass Valid => new("valid");
        public static PseudoClass Invalid => new("invalid");
        public static PseudoClass InRange => new("in-range");
        public static PseudoClass OutOfRange => new("out-of-range");
        public static PseudoClass Required => new("required");
        public static PseudoClass Optional => new("optional");
        public static PseudoClass UserValid => new("user-valid");
        public static PseudoClass UserInvalid => new("user-invalid");

        /* -------------------------
         * Resource / media state
         * ------------------------- */
        public static PseudoClass Playing => new("playing");
        public static PseudoClass Paused => new("paused");
        public static PseudoClass Seeking => new("seeking");
        public static PseudoClass Buffering => new("buffering");
        public static PseudoClass Stalled => new("stalled");
        public static PseudoClass Muted => new("muted");
        public static PseudoClass VolumeLocked => new("volume-locked");

        /* -------------------------
         * Temporal
         * ------------------------- */
        public static PseudoClass Current => new("current");
        public static PseudoClass Past => new("past");
        public static PseudoClass Future => new("future");

        /* -------------------------
         * Element & display state
         * ------------------------- */
        public static PseudoClass Defined => new("defined");
        public static PseudoClass Open => new("open");
        public static PseudoClass Modal => new("modal");
        public static PseudoClass Fullscreen => new("fullscreen");
        public static PseudoClass PopoverOpen => new("popover-open");
        public static PseudoClass PictureInPicture => new("picture-in-picture");

        /* -------------------------
         * Shadow DOM / components
         * ------------------------- */
        public static PseudoClass Host => new("host");

        internal protected override string Render() => $":{Name}";
    }

    /// <summary>
    /// Represents a CSS functional pseudo-class selector, which can take arguments.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Arguments"></param>
    public sealed record FunctionalPseudoClass(string Name, IEnumerable<SelectorExpression> Arguments) : SelectorExpression
    {
        /* -------------------------
         * Functional pseudo-classes
         * ------------------------- */
        public static FunctionalPseudoClass Has(params SelectorExpression[] args) => new("has", args);
        public static FunctionalPseudoClass Not(params SelectorExpression[] args) => new("not", args);
        public static FunctionalPseudoClass Is(params SelectorExpression[] args) => new("is", args);
        public static FunctionalPseudoClass Where(params SelectorExpression[] args) => new("where", args);
        public static FunctionalPseudoClass Lang(string code) => new("lang", new SelectorExpression[] { new RawSelector(code) });
        public static FunctionalPseudoClass Dir(string direction) => new("dir", new SelectorExpression[] { new RawSelector(direction) });

        public static FunctionalPseudoClass NthChild(string expr) => new("nth-child", new[] { new RawSelector(expr) });
        public static FunctionalPseudoClass NthLastChild(string expr) => new("nth-last-child", new[] { new RawSelector(expr) });
        public static FunctionalPseudoClass NthOfType(string expr) => new("nth-of-type", new[] { new RawSelector(expr) });
        public static FunctionalPseudoClass NthLastOfType(string expr) => new("nth-last-of-type", new[] { new RawSelector(expr) });

        /* -------------------------
         * Shadow DOM / components
         * ------------------------- */
        public static FunctionalPseudoClass HostSelector(params SelectorExpression[] args) => new("host", args);
        public static FunctionalPseudoClass HostContext(params SelectorExpression[] args) => new("host-context", args);
        public static FunctionalPseudoClass HasSlotted(params SelectorExpression[] args) => new("has-slotted", args);



        internal protected override string Render()
            => $":{Name}({string.Join(", ", Arguments.Select(a => a.Render()))})";
    }

    /// <summary>
    /// Represents a CSS parent selector, which selects the parent of the current element.
    /// </summary>
    public sealed record ParentSelector() : SelectorExpression
    {
        public static ParentSelector Parent => new();
        internal protected override string Render() => "&";
    }

    /// <summary>
    /// Represents a CSS pseudo-element selector.
    /// </summary>
    /// <param name="Name"></param>
    public sealed record PseudoElement(string Name) : SelectorExpression
    {
        /* -------------------------
         * Pseudo-elements
         * ------------------------- */
        public static PseudoElement Before => new("before");
        public static PseudoElement After => new("after");
        public static PseudoElement FirstLine => new("first-line");
        public static PseudoElement FirstLetter => new("first-letter");
        public static PseudoElement Selection => new("selection");
        public static PseudoElement Marker => new("marker");
        public static PseudoElement Placeholder => new("placeholder");
        public static PseudoElement FileSelectorButton => new("file-selector-button");
        public static PseudoElement Backdrop => new("backdrop");
        public static PseudoElement Cue => new("cue");
        public static PseudoElement Slotted(string name) => new($"slotted({name})");

        internal protected override string Render() => $"::{Name}";
    }

    /// <summary>
    /// Represents a raw CSS selector expression.
    /// </summary>
    /// <param name="Raw"></param>
    public sealed record RawSelector(string Raw) : SelectorExpression
    {
        internal protected override string Render() => Raw;
    }

    /// <summary>
    /// Represents a binary selector expression, which combines two selectors with an operator.
    /// </summary>
    /// <param name="Left"></param>
    /// <param name="Operator"></param>
    /// <param name="Right"></param>
    public sealed record BinarySelector(SelectorExpression Left, string Operator, SelectorExpression Right)
        : SelectorExpression
    {
        internal protected override string Render()
        {
            if (Operator == " ")
                return $"{Left.Render()} {Right.Render()}";
            return $"{Left.Render()}{Operator}{Right.Render()}";
        }
    }

    /// <summary>
    /// Represents a group of selectors in CSS.
    /// </summary>
    /// <param name="Selectors"></param>
    public sealed record GroupedSelector(IEnumerable<SelectorExpression> Selectors) : SelectorExpression
    {
        internal protected override string Render() => string.Join(", ", Selectors.Select(s => s.Render()));
    }
}
