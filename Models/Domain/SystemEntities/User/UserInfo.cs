namespace MKLUODDD.Model.Domain {

    public class UserInfo {

        public Name Name { get; set; }
        public Username Username { get; set; }

        public UserInfo(
            Name name,
            Username username
        ) {
            Name = name;
            Username = username;
        }
    }

    public class UserToUserInfoMapper : IInfoMapper<User, UserInfo> {

        public UserInfo GetInfoOf(User entity) => new UserInfo(entity.Name, entity.Username);
    }
}