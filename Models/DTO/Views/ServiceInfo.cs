namespace MKLUODDD.Model.DTO.Views {

    public class ServiceInfo : AppOutput {

        public override OutputType Type => OutputType.ServiceInfo;
        public override object Payload => throw new System.NotImplementedException();

        public string? id;
        public string? group;
        public string? name;

        public ServiceInfo() {}
        // public ServiceInfo(Domain.Service service)
        // {
        //     throw new System.NotImplementedException();
        // }

    }
}