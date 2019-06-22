using System;

namespace MKLUODDD.Model.Domain {

    public class PasswordSalt : ByteArray {

        public PasswordSalt(byte[] data) : base(data) { }

        public static PasswordSalt GernerateSalt() {
            var salt = new byte[128];
            new Random().NextBytes(salt);
            return new PasswordSalt(salt);
        }
    }
}