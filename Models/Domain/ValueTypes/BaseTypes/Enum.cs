
namespace MKLUODDD.Model.Domain {

    public class Enum : TypeSafeValueType<Enum>, IValue<int> {

        public virtual int Value { get; }

        public Enum(int value) {
            this.Value = value;
        }
        protected override bool ValueEquals(object obj) {
            return Value == (obj as Enum)?.Value;
        }

        public override string ToString() => Value.ToString();

        public override int GetHashCode() => Value.GetHashCode();
    }
}