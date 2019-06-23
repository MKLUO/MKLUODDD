namespace MKLUODDD.Model.Domain {

    public class Float : TypeSafeValueType<Float>, IValue<double> {

        public virtual double Value { get; }

        public Float(double value) {
            this.Value = value;
        }

        public static implicit operator Float(double value) =>
            new Float(value);
        
        protected override bool ValueEquals(object obj) {
            return Value == (obj as Float)?.Value;
        }

        public override string ToString() => Value.ToString();

        public override int GetHashCode() => Value.GetHashCode();
    }
}