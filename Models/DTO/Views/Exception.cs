
namespace MKLUODDD.Model.DTO.Views {

    public class Exception : AppOutput {

        public override OutputType Type => OutputType.Exception;
        public override object Payload => new { Messege };

        public string? Messege { get; }
        
        public Exception(string messege) {
            this.Messege = messege;
        }
    }
}