namespace MKLUODDD.Model.Domain {

    public class Int : TypeSafeValueType<Int>, IValue<int> {

        public virtual int Value { get; }

        public Int(int value) {
            this.Value = value;
        }

        public static implicit operator Int(int value) =>
            new Int(value);
        
        protected override bool ValueEquals(object obj) {
            return Value == (obj as Int)?.Value;
        }

        public override string ToString() => Value.ToString();

        public override int GetHashCode() => Value.GetHashCode();
    }
}