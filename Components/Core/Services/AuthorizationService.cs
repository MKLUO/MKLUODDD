namespace MKLUODDD.Core {

    using Context;
    using Model.Persist;
    using Model.Domain;

    public class AuthorizationResult {

        public enum ResultType {
            Success,
            Failed,
            Error
        }

        public ResultType Type { get; }

        // public int? UserId { get; }
        // public string Messege { get; }

        public AuthorizationResult(ResultType type) {
            this.Type = type;
        }

        public static AuthorizationResult Success() =>
            new AuthorizationResult(ResultType.Success);

        public static AuthorizationResult Failed() =>
            new AuthorizationResult(ResultType.Failed);

        public static AuthorizationResult Error() =>
            new AuthorizationResult(ResultType.Error);
    }

    public class AuthorizationService {

        IContextHandle Handle { get; }

        IPersistContext<User> UserContext { get; }

        public AuthorizationService(
            IContextHandle handle,
            IPersistContext<User> userContext
        ) {
            this.Handle = handle;
            this.UserContext = userContext;
        }

        public AuthorizationResult Authorize(
            PersistInfo<User> userInfo,
            ServiceAction action
        ) {
            var user = UserContext.GetEntity(userInfo);
            if (user == null)
                return AuthorizationResult.Error();

            if (user.Claims(action))
                return AuthorizationResult.Success();
            else 
                return AuthorizationResult.Failed();
        }
    }
}