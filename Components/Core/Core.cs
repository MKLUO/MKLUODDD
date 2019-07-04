using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

// NOTE: Here is the paradise of pure Business/Domain logic!
// Keep in mind that no App/DAL logic belongs here.
// Remember to factor out repeatitive functionality into separate class/files.

namespace MKLUODDD.Core {
    using Model.Persist;
    using Model.Domain;

    // public class CrmCore : ICore {
    public class Core {

        IServiceProvider ServiceProvider { get; }
        public Core(
            IServiceProvider serviceProvider
        ) {
            this.ServiceProvider = serviceProvider;
        }

        public AuthenticationResult Authenticate(
            Username username, Password password) {

            return ServiceProvider.GetRequiredService<IAuthenticationService>()
                .Authenticate(username, password);
        }

        // public AuthorizationResult Authorize(
        //     PersistInfo<User> userInfo,
        //     ServiceAction action) { 

        //     return ServiceProvider.GetRequiredService<IAuthorizationService>()
        //         .Authorize(userInfo, action);
        // }

        // public List<ServiceKey> GetAvailableServices(int userId) {
        //     return ServiceProvider.GetRequiredService<AuthorizationService>()
        //         .AvailableServices(userId);
        // }

        // TODO: ServiceKey: value type; ServiceEntry: entity.

        // public ServiceEntry GetService(ServiceKey key) {

        // }
    }
}