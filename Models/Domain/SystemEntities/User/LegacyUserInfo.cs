namespace MKLUODDD.Model.Domain {
    
    public class LegacyUser : BaseUser, IUser {

        public Name Name { get; }
        public Username Username { get; }
        public Password Password { get; }

        public LegacyUser(
            Name name,
            Username username,
            Password password,
            UserMiscData miscData
        ) {
            Name = name;
            Username = username;
            Password = password;
            MiscData = miscData;
        }

        public bool HasPassword(Password password) =>
            this.Password.Equals(password, typeCheck: true);
    }
}