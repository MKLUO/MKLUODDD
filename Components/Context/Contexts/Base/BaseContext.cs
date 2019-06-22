namespace MKLUODDD.Context
{

    using DAL;
    using Mapper;
    using Model.Persist;

    public abstract class BaseContext<T, TD> : 
        BaseReadContext<T, TD>,
        IReadContext<T>,
        IAggregationContext<T, TD>,
        IPersistContext<T>
        where T : class
        where TD : class, new() 
    {
        protected BaseContext(IBaseRepository<TD> repo, IReadonlyMapper<T, TD> mapper, IDomainMapper domainMapper) : base(repo, mapper, domainMapper) { }

        public PersistInfo<T>? GetPersistInfo(in T entity) {
            if (Store.ContainsKey(entity))
                return Mapper.InfoOf(Store[entity]);
            else return null;
        }

        public T? GetEntity(in PersistInfo<T> info, bool force = false) {
            var obj = Repo.GetById(info.Id);
            if (obj == null) return null;

            if ((Mapper.InfoOf(obj).Ver != info.Ver) && !force)
                return null;

            return Pull(obj);
        }

        protected virtual void PullMidware(in T entity, in TD obj, bool ignoreCollection = false) { }

        public override T Pull(in TD obj, bool halt = false) {
            if (StoreLookUp.ContainsKey(obj))
                return StoreLookUp[obj];

            var entity = Mapper.Compose(obj);
            Attach(entity, obj);

            PullMidware(entity, obj, ignoreCollection: halt);

            return entity;
        }

        public override void Draw(in T entity) {
            if (!Store.ContainsKey(entity))
                return;

            PullMidware(entity, Store[entity]);
        }

        protected virtual void PushMidware(in T entity, in TD obj) { }
        public override abstract TD? Push(in T entity);
    }
}