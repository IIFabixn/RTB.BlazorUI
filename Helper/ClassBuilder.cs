using RTB.BlazorUI.Services.Theme;
using System.Text;

namespace RTB.BlazorUI.Helper
{
    /// <summary>
    /// Little Helper Class to build a class string without unnecessary spaces.
    /// 
    /// NOTE: Calling <see cref="Build"/> is required in order to get the final string.
    /// </summary>
    public class ClassBuilder
    {
        private readonly StringBuilder _builder;
        private ClassBuilder(string? inital = "")
        {
            _builder = new(inital);
        }

        public static ClassBuilder Create(params string?[]? initals)
        {
            if (initals is null) return new ClassBuilder();

            return new ClassBuilder(string.Join(' ', initals.Where(v => !string.IsNullOrWhiteSpace(v))));
        }

        public ClassBuilder Append(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _builder.Append(' ');
                _builder.Append(name);
            }

            return this;
        }

        public ClassBuilder Append(params string?[] names)
        {
            if (names is { Length: 0 }) return this;

            Append(string.Join(' ', names));

            return this;
        }

        public ClassBuilder Merge(ClassBuilder? otherBuilder)
        {
            if (otherBuilder is null) return this;

            Append(otherBuilder?.Build());

            return this;
        }

        public ClassBuilder AppendIf(string? name, Func<bool> condition)
        {
            return condition() ? Append(name) : this;
        }

        public ClassBuilder AppendIf(string? name, bool condition)
        {
            return condition ? Append(name) : this;
        }
        public ClassBuilder AppendIfElse(string? trueValue, string? falseValue, bool condition)
        {
            return condition ? Append(trueValue) : Append(falseValue);
        }

        public string Build()
        {
            return _builder.ToString().Trim();
        }
    }
}
