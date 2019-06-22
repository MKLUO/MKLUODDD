namespace MKLUODDD.Context
{

    using DAL;
    using Mapper;

    public class ReadonlyBasicContext<T, TD> : 
        BaseContext<T, TD>, 
        IReadContext<T>,
        IAggregationContext<T, TD>,
        IPersistContext<T>
        where T : class
        where TD : class, new() 
    {

        public ReadonlyBasicContext(
            IBaseRepository<TD> repo,
            IReadonlyMapper<T, TD> mapper,
            IDomainMapper domainMapper) : base(repo, mapper, domainMapper) { }

        public override TD? Push(in T entity) {
            if (!Store.ContainsKey(entity))
                return null;

            var obj = Store[entity];

            PushMidware(entity, obj);

            return obj;
        }
    }
}