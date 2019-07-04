namespace MKLUODDD.Core
{
    using Model.Persist;
    using Model.Domain;

    public class UserPersistInfo : PersistInfo<object> {

        public UserPersistInfo(int id) : base(id) { }
        public UserPersistInfo(int id, ByteArray ver) : base(id, ver) { }
        public static implicit operator UserPersistInfo((int id, ByteArray ver) info) =>
            new UserPersistInfo(info.id, info.ver);
    }
}