namespace MKLUODDD.Model.DTO.Views {

    public class LoginResult : AppOutput {

        public override OutputType Type => OutputType.LoginInfo;
        public override object Payload => new { Messege };

        public string? Messege { get; }
        
        public LoginResult(string messege) {
            this.Messege = messege;
        }
    }
}