namespace MKLUODDD.Model.Domain {

    public class ServiceAction : String {
        
        public ServiceAction(string str) : base(str) { }
        public static implicit operator ServiceAction(string str) => new ServiceAction(str);
    }
}