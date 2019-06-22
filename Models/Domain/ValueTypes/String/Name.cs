namespace MKLUODDD.Model.Domain {

    public class Name : String {
        
        public Name(string str) : base(str) { }
        public static implicit operator Name(string str) => new Name(str);
    }
}