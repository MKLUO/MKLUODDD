namespace MKLUODDD.Model.Domain {

    public class Password : CompactString {

        public Password(string str) : base(str) { }
        public static implicit operator Password(string str) => new Password(str);
    }
}