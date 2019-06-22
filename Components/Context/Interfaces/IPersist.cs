

namespace MKLUODDD.Model.ORM
{

    public interface IPersist {
        int Id { get; }
        byte[] Version { get; }
    }
}