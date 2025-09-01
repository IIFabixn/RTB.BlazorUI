using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTB.Blazor.Styled.Core
{
    public sealed class StyleBuilder
    {
        // base declarations (for the root selector)
        public DeclarationSet Base { get; } = [];

        public static StyleBuilder Start => new();

        // all other fragments (selectors, groups, keyframes)
        private readonly List<IStyleFragment> _fragments = [];
        private readonly List<IStyleContributor> _contributors = new();
        private readonly object _gate = new();
        public void Register(IStyleContributor c)
        {
            if (c is null) return;
            lock (_gate) _contributors.Add(c);
        }

        public void Unregister(IStyleContributor c)
        {
            if (c is null) return;
            lock (_gate) _contributors.Remove(c);
        }

        /// Compose the builder from all registered contributors.
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

        public void AddFragment(IStyleFragment f)
        {
            if (f != null) _fragments.Add(f);
        }

        // Sugar for primitives
        public StyleBuilder Set(string prop, string value)
        {
            Base.Add(prop, value);
            return this;
        }

        public StyleBuilder SetIfNotNull(string? prop, string? value)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value)) return this;
            return Set(prop, value);
        }

        public StyleBuilder SetIf(string? prop, string? value, Func<bool> Condition)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(value) || !Condition()) return this;
            return Set(prop, value);
        }

        public StyleBuilder SetIf(string? prop, string? value, bool Condition) => SetIf(prop, value, () => Condition);


        // Sugar: nested selector
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

        // Sugar: group rules
        public GroupRule Media(string prelude, Action<StyleBuilder> build)
            => Group("@media", prelude, build);
        public GroupRule Supports(string prelude, Action<StyleBuilder> build)
            => Group("@supports", prelude, build);
        public GroupRule Container(string prelude, Action<StyleBuilder> build)
            => Group("@container", prelude, build);

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

        private Keyframes FindOrAddKeyframes(string name)
        {
            for (int i = 0; i < _fragments.Count; i++)
                if (_fragments[i] is Keyframes k && string.Equals(k.Name, name, StringComparison.Ordinal))
                    return k;
            var created = new Keyframes(name);
            _fragments.Add(created);
            return created;
        }

        public Keyframes Keyframes(string name, Action<Keyframes> build)
        {
            var kf = FindOrAddKeyframes(name); // <- merge-by-name
            build?.Invoke(kf);
            return kf;
        }

        public string BuildScoped(string className)
        {
            if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException(nameof(className));
            var root = "." + className.TrimStart('.');

            var sb = new StringBuilder(512);
            var w = new ScopedWriter(sb, root);

            // base
            Base.Emit(w);

            // others
            foreach (var f in _fragments) f.Emit(w);

            return sb.ToString();
        }

        public StyleBuilder Absorb(StyleBuilder other)
        {
            if (other is null) return this;
            Base.Join(other.Base);
            foreach (var f in GetFragments(other)) // expose a typed iterator or method
                AddFragment(f);
            return this;
        }

        private static IEnumerable<IStyleFragment> GetFragments(StyleBuilder other)
        {
            if (other is null) yield break;
            foreach (var f in other._fragments)
                yield return f;
        }

        public StyleBuilder ClearAll()
        {
            Base.Clear();
            _fragments.Clear();
            return this;
        }
    }
}
