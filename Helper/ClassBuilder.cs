using System.Text;

namespace RTB.BlazorUI.Helper
{
    public class ClassBuilder
    {
        private readonly StringBuilder _builder;
        private ClassBuilder(string? inital = "")
        {
            _builder = new(inital);
        }

        public static ClassBuilder Create(string? inital = "")
        {
            return new ClassBuilder(inital);
        }

        public ClassBuilder Append(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _builder.Append(' ');
                _builder.Append(name);
            }

            return this;
        }

        public ClassBuilder AppendIf(string name, Func<bool> condition)
        {
            return condition() ? Append(name) : this;
        }

        public ClassBuilder AppendIf(string name, bool condition)
        {
            return condition ? Append(name) : this;
        }

        public string Build()
        {
            return _builder.ToString().Trim();
        }

        public override string ToString()
        {
            return Build();
        }
    }
}
