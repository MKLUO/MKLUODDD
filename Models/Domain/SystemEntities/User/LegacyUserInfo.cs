namespace MKLUODDD.Model.Domain {
    
    public class LegacyUserInfo : BaseUser, IUser {

        public Name Name { get; }
        public Username Username { get; }
        public Password Password { get; }

        public LegacyUserInfo(
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