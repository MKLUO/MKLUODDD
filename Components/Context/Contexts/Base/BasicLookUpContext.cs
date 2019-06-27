using System.Collections.Generic;

namespace MKLUODDD.Context
{

    using MKLUODDD.Context;
    using MKLUODDD.DAL;
    using MKLUODDD.Mapper;
    using MKLUODDD.Model.ORM;

    public class BasicLookUpContext<TGroup, TORMGroup, T, TORM> : BasicContext<TGroup, TORMGroup>, ILookUpContext<T, TGroup> 
    where TGroup : class
    where TORMGroup : class, INavSome<TORM>, new()
    where TORM : class, new() {

        protected IAggregationContext<T, TORM> AggrContext { get; }

        public BasicLookUpContext(
            IWriteRepository<TORMGroup> repo, 
            IMapper<TGroup, TORMGroup> mapper, 
            IDomainMapper domainMapper,
            IAggregationContextProxy<T, TORM> aggrContext
        ) : base(repo, mapper, domainMapper) {
            this.AggrContext = aggrContext;
        }

        public virtual IEnumerable<T> WhoLookUp(TGroup group) { 
            if (Store.ContainsKey(group))
                foreach (var item in Store[group].NavSome(new TORM()).Get())
                    yield return AggrContext.Pull(item, ignoreCollection: true);
            else yield break;
        }        
    }
}