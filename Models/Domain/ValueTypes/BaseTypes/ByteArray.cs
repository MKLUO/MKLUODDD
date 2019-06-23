using System.Linq;
using System.Text;

namespace MKLUODDD.Model.Domain {

    public class ByteArray : TypeSafeValueType<ByteArray> {

        byte[] DataInstance { get; }
        public byte[] Data => DataInstance.Clone() as byte[] ??
        throw new InvalidValueException();

        public ByteArray(byte[] data) {
            this.DataInstance = data.Clone() as byte[] ??
                throw new InvalidValueException();
        }

        public static implicit operator ByteArray(byte[] data) =>
            new ByteArray(data);

        protected override bool ValueEquals(object obj) {
            return Data.SequenceEqual((obj as ByteArray)?.Data);
        }

        public override string ToString() => Encoding.ASCII.GetString(Data);

        public override int GetHashCode() => Data.GetHashCode();
    }
}