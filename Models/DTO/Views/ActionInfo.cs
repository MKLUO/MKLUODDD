namespace MKLUODDD.Model.DTO.Views {

    public class ActionInfo : AppOutput {

        public override OutputType Type => OutputType.ActionInfo;
        public override object Payload => throw new System.NotImplementedException();

        public string? id;
        public string? group;
        public string? name;

        public ActionInfo() {}
        // public ServiceInfo(Domain.Service service)
        // {
        //     throw new System.NotImplementedException();
        // }

    }
}