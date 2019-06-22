
using System.Reflection;

namespace MKLUODDD.Model.Domain {

    public abstract class ValueType<T> where T : ValueType<T> {

        public override abstract bool Equals(object obj);

        public override abstract int GetHashCode();

        public virtual bool NotEquals(object obj) => !Equals(obj);

        public static bool operator ==(ValueType<T> left, ValueType<T> right) {
            return left.Equals(right);
        }

        public static bool operator !=(ValueType<T> left, ValueType<T> right) {
            return !left.Equals(right);
        }

        public class InvalidValueException : System.Exception { }
    }

    public interface IValue<T> {
        T Value { get; }
    }
}