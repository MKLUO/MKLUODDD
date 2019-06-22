using System.Security.Cryptography;
using System.Text;

namespace MKLUODDD.Model.Domain {

    public class PasswordHash : ByteArray {

        public PasswordHash(byte[] data) : base(data) { }

        public static PasswordHash Hash(Password password, PasswordSalt salt) {
            using var hmac = new HMACSHA512(salt.Data);
            return new PasswordHash(
                hmac.ComputeHash(
                    Encoding.ASCII.GetBytes(password.Value)));
        }
    }
}