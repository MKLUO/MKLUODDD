using System.Reflection;

namespace MKLUODDD.Model.Domain {

    public class String : TypeSafeValueType<String>, IValue<string> {

        public virtual string Value { get; }

        public String(string str) {
            this.Value = str;
        }
        protected override bool ValueEquals(object obj) {
            return Value == (obj as String)?.Value;
        }

        public bool Contains(String str) =>
            Value.Contains(str.Value);

        public bool StartsWith(String str) =>
            Value.StartsWith(str.Value);

        public override string ToString() => Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}