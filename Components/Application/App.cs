using System;
using Newtonsoft.Json.Linq;

namespace MKLUODDD.Apps {

    using Core;
    using Model.Persist;
    using Model.Domain;
    using Model.DTO.Forms;
    using Model.DTO.Views;

    public class App {

        AppSession Session { get; }
        Core Core { get; }

        public App(
            Core core, 
            AppSession session) {
            this.Core = core;
            this.Session = session;
        }

        public LoginResult Authenticate(LoginForm user) {

            // TODO: Form should provide "form format" info.

            var authResult = Core.Authenticate(
                new Username(user.Username),
                new Password(user.Password)
            );

            switch (authResult.Type) {
                case AuthenticationResult.ResultType.Success:
                    Session.User = authResult.UserInfo;
                    Session.TimeCreated = DateTime.Now.ToString();
                    break;
                default:
                    Session.Clear();
                    break;
            }
            return new LoginResult(authResult);
        }

        public bool IsLoggedIn() => Session.User != null;

        
        public ServiceInfo GetServiceInfos() {

            return new ServiceInfo();
        }

        public ServiceInfo GetServiceInfo(string serviceKey) {

            return new ServiceInfo();
        }

        public ActionInfo GetActionInfo(string serviceKey, string action) {

            return new ActionInfo();
        }

        public ServiceOutput Run(string serviceKey, string actionKey, JObject payload) {

            // AuthorizationResult authResult;

            // if (Core.GetService(serviceKey)?.GetAction(actionKey)
            //         is ServiceAction action &&
            //     Session.User is PersistInfo<User> user)
            //     authResult = Core.Authorize(user, action);
            // else 
            //     authResult = AuthorizationResult.Error();

            // switch (authResult.Type) {
            //     case AuthenticationResult.ResultType.Success:
            //         Session.User = authResult.UserInfo;
            //         Session.TimeCreated = DateTime.Now.ToString();
            //         break;
            //     default:
            //         Session.Clear();
            //         break;
            // }

            return new ServiceOutput();
        }
    }
}