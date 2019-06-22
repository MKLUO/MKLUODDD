namespace MKLUODDD.Model.Domain {
    
    public class Username : CompactString {

        public Username(string str) : base(str) { }
        public static implicit operator Username(string str) => new Username(str);
    }
}