namespace MKLUODDD.Model.DTO.Views {

    public class ServiceOutput : AppOutput {

        public override OutputType Type => OutputType.ServiceOutput;
        public override object Payload => throw new System.NotImplementedException();

        public string? id;
        public string? group;
        public string? name;

        public ServiceOutput() {}
        // public ServiceInfo(Domain.Service service)
        // {
        //     throw new System.NotImplementedException();
        // }

    }
}