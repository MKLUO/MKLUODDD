using System.Reflection;

namespace MKLUODDD.Model.Domain {

    public class Id : TypeSafeValueType<Id>, IValue<int> {

        public virtual int Value { get; }

        public Id(int id) {
            this.Value = id;
        }

        protected override bool ValueEquals(object obj) {
            return Value == (obj as Id)?.Value;
        }
        
        public override string ToString() => Value.ToString();

        public override int GetHashCode() => Value.GetHashCode();
    }
}