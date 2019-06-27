
namespace MKLUODDD.Context {

    using DAL;
    using Mapper;

    public class BasicView<T, TD>:
        BaseReadContext<T, TD>,
        IReadContext<T>,
        ISubscriberContext<TD>
        where T : class
    where TD : class, new() {
        protected new IViewMapper<T, TD> Mapper { get; }

        public BasicView(IReadRepository<TD> repo, IViewMapper<T, TD> mapper, IDomainMapper domainMapper) : base(repo, mapper, domainMapper) {
            Mapper = mapper;
        }

        public void Update(TD obj) {
            if (!StoreLookUp.ContainsKey(obj))
                return;

            Mapper.ComposeInplace(StoreLookUp[obj], obj);
        }

        public override TD PushTransient(in T entity)
        {
            throw new System.NotImplementedException();
        }

        public override void AttachTransient(in T entity, in TD obj)
        {
            throw new System.NotImplementedException();
        }
    }
}