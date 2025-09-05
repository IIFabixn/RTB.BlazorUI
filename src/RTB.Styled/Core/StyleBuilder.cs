using RTB.Blazor.Styled.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    /// <summary>
    /// A builder for CSS styles, supporting base declarations, nested selectors, groups (media/supports/container), and keyframes.
    /// </summary>
    public sealed class StyleBuilder
    {
        /// <summary>
        /// Base declarations (the root level, no selector).
        /// </summary>
        public DeclarationSet Base { get; } = [];

        /// <summary>
        /// Create a new StyleBuilder instance.
        /// </summary>
        public static StyleBuilder Start => new();

        // all other fragments (selectors, groups, keyframes)
        private readonly List<IStyleFragment> _fragments = [];
        private readonly List<IStyleContributor> _contributors = [];
        private readonly object _gate = new();

        /// <summary>
        /// Register a style contributor to participate in the next composition.
        /// </summary>
        /// <param name="c"></param>
        public void Register(IStyleContributor c)
        {
            if (c is null) return;
            lock (_gate) _contributors.Add(c);
        }

        /// <summary>
        /// Unregister a style contributor.
        /// </summary>
        /// <param name="c"></param>
        public void Unregister(IStyleContributor c)
        {
            if (c is null) return;
            lock (_gate) _contributors.Remove(c);
        }

        /// <summary>
        /// Compose the style by clearing existing declarations and fragments,
        /// </summary>
        public void Compose()
        {
            // start fresh on each compose (render)
            Base.Clear();
            _fragments.Clear();

            IStyleContributor[] snapshot;
            lock (_gate) snapshot = [.. _contributors];

            foreach (var c in snapshot)
                c.Contribute(this);
        }

        /// <summary>
        /// Add a style fragment (selector, group, keyframes).
        /// </summary>
        /// <param name="f"></param>
        public void AddFragment(IStyleFragment f)
        {
            if (f != null) _fragments.Add(f);
        }

        /// <summary>
        /// Set a CSS property to a value in the base declaration set.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public StyleBuilder Set(string prop, string value)
        {
            Base.Add(prop, value);
            return this;
        }

        /// <summary>
        /// Set a CSS property to a value in the base declaration set if both are not null or whitespace.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public StyleBuilder SetIfNotNull(string? prop, string? value)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value)) return this;
            return Set(prop, value);
        }

        /// <summary>
        /// Set a CSS property to a value in the base declaration set if both are not null or whitespace and the condition is true.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public StyleBuilder SetIf(string? prop, string? value, Func<bool> Condition)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value) || !Condition()) return this;
            return Set(prop, value);
        }

        /// <summary>
        /// Set a CSS property to a value in the base declaration set if both are not null or whitespace and the condition is true.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="Condition"></param>
        /// <returns></returns>
        public StyleBuilder SetIf(string? prop, string? value, bool Condition) => SetIf(prop, value, () => Condition);

        /// <summary>
        /// Add a selector rule with nested declarations and optional child fragments.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public SelectorRule Selector(string selector, Action<StyleBuilder> build)
        {
            var sb = new StyleBuilder();
            build?.Invoke(sb);
            var rule = new SelectorRule(selector)
            {
                Declarations = sb.Base
            };
            rule.Children.AddRange(sb._fragments);
            _fragments.Add(rule);
            return rule;
        }

        /// <summary>
        /// Add a group rule (media, supports, container) with nested declarations and optional child fragments.
        /// </summary>
        /// <param name="prelude"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public GroupRule Media(string prelude, Action<StyleBuilder> build)
            => Group("@media", prelude, build);

        /// <summary>
        /// Add a group rule (media, supports, container) with nested declarations and optional child fragments.
        /// </summary>
        /// <param name="prelude"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public GroupRule Supports(string prelude, Action<StyleBuilder> build)
            => Group("@supports", prelude, build);

        /// <summary>
        /// Add a group rule (media, supports, container) with nested declarations and optional child fragments.
        /// </summary>
        /// <param name="prelude"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public GroupRule Container(string prelude, Action<StyleBuilder> build)
            => Group("@container", prelude, build);

        /// <summary>
        /// Add a group rule (media, supports, container) with nested declarations and optional child fragments.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="prelude"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public GroupRule Group(string kind, string prelude, Action<StyleBuilder> build)
        {
            var sb = new StyleBuilder();
            build?.Invoke(sb);
            var g = new GroupRule(kind, prelude);
            if (!sb.Base.IsEmpty) g.Children.Add(sb.Base);
            g.Children.AddRange(sb._fragments);
            _fragments.Add(g);
            return g;
        }

        /// <summary>
        /// Find an existing keyframes block by name or create a new one.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Keyframes FindOrAddKeyframes(string name)
        {
            for (int i = 0; i < _fragments.Count; i++)
                if (_fragments[i] is Keyframes k && string.Equals(k.Name, name, StringComparison.Ordinal))
                    return k;
            var created = new Keyframes(name);
            _fragments.Add(created);
            return created;
        }

        /// <summary>
        /// Add or modify a keyframes block by name with nested frames.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public Keyframes Keyframes(string name, Action<Keyframes> build)
        {
            var kf = FindOrAddKeyframes(name); // <- merge-by-name
            build?.Invoke(kf);
            return kf;
        }

        /// <summary>
        /// A token used to scope the CSS to a specific class name.
        /// </summary>
        public const string SCOPE_TOKEN = "__rtb_scope__";

        /// <summary>
        /// Build the complete CSS style as a string, scoped to the provided class name.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public (string cls, string style) BuildScoped(string? className = null)
        {
            var root = "." + (string.IsNullOrWhiteSpace(className) ? SCOPE_TOKEN : className);

            var sb = new StringBuilder(512);
            var w = new ScopedWriter(sb, root);

            // base
            Base.Emit(w);

            // others
            foreach (var f in _fragments) f.Emit(w);

            string cls;
            string css = sb.ToString();

            if (string.IsNullOrEmpty(className))
            {
                var hash = CssHasher.Hash(css);
                cls = $"rtb-{hash:X}";
                css = css.Replace(SCOPE_TOKEN, cls);
            }
            else
            {
                cls = className;
            }

            return (cls, css);
        }

        /// <summary>
        /// Absorb another StyleBuilder's base declarations and fragments into this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public StyleBuilder Absorb(StyleBuilder other)
        {
            if (other is null) return this;
            Base.Join(other.Base);
            foreach (var f in GetFragments(other)) // expose a typed iterator or method
                AddFragment(f);
            return this;
        }

        /// <summary>
        /// Clear all base declarations and fragments.
        /// </summary>
        /// <returns></returns>
        public StyleBuilder ClearAll()
        {
            Base.Clear();
            _fragments.Clear();
            return this;
        }

        private static IEnumerable<IStyleFragment> GetFragments(StyleBuilder other)
        {
            if (other is null) yield break;
            foreach (var f in other._fragments)
                yield return f;
        }
    }
}
