namespace MKLUODDD.Core
{
    using Model.Domain;

    public interface IAuthenticationService {
        AuthenticationResult Authenticate(
            Username username,
            Password password);
    }
}