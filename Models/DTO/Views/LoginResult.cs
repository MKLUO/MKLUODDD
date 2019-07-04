namespace MKLUODDD.Model.DTO.Views {

    using Core;

    public class LoginResult : AppOutput {

        public override OutputType Type => OutputType.LoginInfo;
        public override object Payload { get; }
        
        public LoginResult(AuthenticationResult authResult) {
            this.Payload = new {
                result = authResult.Type.ToString(),
                messege = authResult.Messege
            };
        }
    }
}