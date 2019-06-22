using System.Linq;

namespace MKLUODDD.Core {

    using Context;
    using Model.Persist;
    using Model.Domain;

    public class AuthenticationResult {

        public enum ResultType {
            Success,
            Failed,
            Error
        }

        public ResultType Type { get; }

        public PersistInfo<User>? UserInfo { get; }
        public string Messege { get; }

        public AuthenticationResult(ResultType type, PersistInfo<User>? userInfo, string messege) {
            this.Type = type;
            this.UserInfo = userInfo;
            this.Messege = messege;
        }

        public static AuthenticationResult Success(Name name, PersistInfo<User> userInfo) =>
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
    
    public class AuthenticationService{

        IContext<User> UserContext { get; }
        IPersistContext<User> UserPersist { get; }
        IReadContext<UserInfo> UserView { get; }
        IReadContext<LegacyUser> LegacyUserView { get; }

        IContextHandle Handle { get; }

        public AuthenticationService(
            IContext<User> userContext,
            IPersistContext<User> userPersist,
            IReadContext<UserInfo> userInfoView,
            IReadContext<LegacyUser> legacyUserView,
            IContextHandle handle
        ) {
            this.UserContext = userContext;
            this.UserPersist = userPersist;
            this.UserView = userInfoView;
            this.LegacyUserView = legacyUserView;
            this.Handle = handle;
        }

        public AuthenticationResult Authenticate(
            Username username,
            Password password,
            bool legacy = false) {

            using var domainTransaction = Handle.BeginDomainTransaction();

            int userCount;

            if (legacy)
                userCount = LegacyUserView
                .Count(user => user.Username == username);

            else
                userCount = UserView
                .Count(user => user.Username == username);

            AuthenticationResult result;

            switch (userCount) {
                case 0:
                    if (legacy) 
                        return AuthenticationResult.Failed();

                    else {
                        result = Authenticate(username, password, legacy : true);
                        break;
                    }

                case 1:
                    User user;
                    
                    if (legacy) {
                        var users = LegacyUserView
                        .Query(user => user.Username == username).ToList();

                        if (users[0].HasPassword(password)) {
                            user = User.MigrateFromLegacy(users[0]);
                            UserContext.Add(user);

                        } else return AuthenticationResult.Failed();

                    } else {
                        var users = UserContext
                        .Query(user => user.Username == username).ToList();

                        if (users[0].HasPassword(password)) {
                            user = users[0];

                        } else return AuthenticationResult.Failed();
                    }

                    var info = UserPersist.GetPersistInfo(user);
                    if (info != null) {
                        result = AuthenticationResult.Success(user.Name, info);
                        break;

                    } else return AuthenticationResult.Error();

                default:
                    return AuthenticationResult.Error();
            }

            domainTransaction.Commit();
            return result;
        }
    }
}