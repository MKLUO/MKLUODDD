namespace MKLUODDD.Core
{
    using Model.Domain;

    public class AuthenticationResult {

        public enum ResultType {
            Success,
            Failed,
            Error
        }

        public ResultType Type { get; }

        public UserPersistInfo? UserInfo { get; }
        public string Messege { get; }

        public AuthenticationResult(ResultType type, UserPersistInfo? userInfo, string messege) {
            this.Type = type;
            this.UserInfo = userInfo;
            this.Messege = messege;
        }

        public static AuthenticationResult Success(Name name, UserPersistInfo userInfo) =>
            new AuthenticationResult(
                ResultType.Success, userInfo,
                $"Hello {name.Value}. Welcome to MKLUODDD 2019.");

        public static AuthenticationResult Failed() =>
            new AuthenticationResult(
                ResultType.Failed, null,
                "Incorrect username or password.");

        public static AuthenticationResult Error() =>
            new AuthenticationResult(
                ResultType.Error, null,
                "Authentication error! Please contact administrator.");
    }
}