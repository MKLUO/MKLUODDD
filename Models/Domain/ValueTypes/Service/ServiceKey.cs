namespace MKLUODDD.Model.Domain {

    public class ServiceKey : String {
        public ServiceKey(string name) : base(name) {}

        public static implicit operator ServiceKey(string name) => 
            new ServiceKey(name);
    }
}