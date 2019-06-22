
namespace MKLUODDD.Model.Persist {    
    
    public class PersistInfo<T> {
        public int Id { get; }
        public Domain.ByteArray Ver { get; }

        public PersistInfo(int id, Domain.ByteArray ver) {
            Id = id;
            Ver = ver;
        }

        public PersistInfo(int id) {
            Id = id;
            Ver = new byte[0];
        }
    }
}