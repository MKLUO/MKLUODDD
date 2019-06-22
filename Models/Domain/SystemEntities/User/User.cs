
namespace MKLUODDD.Model.Domain {

    public class User : BaseUser, IUser {

        public Name Name { get; private set; }
        public Username Username { get; }
        public PasswordHash PasswordHash { get; private set; }
        public PasswordSalt PasswordSalt { get; private set; }

        public LegacyUser? Predecessor { get; private set; }

        public User(
            Name name,
            Username username,
            PasswordHash passwordHash,
            PasswordSalt passwordSalt,
            UserMiscData? miscData = null,

            LegacyUser? predecessor = null
        ) {
            Name = name;
            Username = username;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            MiscData = miscData ?? UserMiscData.None;

            Predecessor = predecessor;
        }

        public void SetPredecessor(LegacyUser? preUser) {
            Predecessor = preUser;
            // return this;
        }

        public static User NewUser(
            Name name,
            Username username,
            Password password
        ) {
            var salt = PasswordSalt.GernerateSalt();
            return new User(
                name,
                username,
                PasswordHash.Hash(password, salt),
                salt
            );
        }

        public bool HasPassword(Password password) =>
            this.PasswordHash.Equals(
                PasswordHash.Hash(password, PasswordSalt));

        public void SetName(Name newName) {
            Name = newName;
        }

        public void ChangePassword(Password newPassword) {
            PasswordSalt = PasswordSalt.GernerateSalt();
            PasswordHash = PasswordHash.Hash(newPassword, PasswordSalt);
        }

        public bool Claims(ServiceAction action) {
            return true;
        }

        public static User MigrateFromLegacy(LegacyUser legacyUser) {
            var salt = PasswordSalt.GernerateSalt();

            return new User(
                name: legacyUser.Name,
                username: legacyUser.Username,
                passwordHash: PasswordHash.Hash(legacyUser.Password, salt),
                passwordSalt : salt,
                miscData : legacyUser.MiscData,
                predecessor : legacyUser
            );
        }
    }    
}