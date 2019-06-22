
using System.Reflection;

namespace MKLUODDD.Model.Domain {

    public abstract class TypeSafeValueType<T> : ValueType<T> where T : TypeSafeValueType<T> {

        public override bool Equals(object obj) => Equals(obj, true);

        public bool Equals(object obj, bool typeCheck) {
            if (typeCheck)
                return (obj is T) && ValueEquals(obj);
            else 
                return ValueEquals(obj);
        }

        public override bool NotEquals(object obj) => NotEquals(obj, false);
        public bool NotEquals(object obj, bool typeCheck) => !Equals(obj, typeCheck);

        public override abstract int GetHashCode();        

        protected abstract bool ValueEquals(object obj);
    }
}