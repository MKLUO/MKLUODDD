using System;
using System.Collections.Generic;

namespace ShiftArr.Context
{

    using MKLUODDD.Context;
    using MKLUODDD.DAL;
    using MKLUODDD.Mapper;
    using MKLUODDD.Model.ORM;

    public abstract class BasicGroupContext<TGroup, T, TORMGroup, TORM> : BasicContext<TGroup, TORMGroup>
    where TORMGroup : class, INavSome<TORM>, new()
    where TGroup : class
    where TORM : class, INav<TORMGroup>, new() {
        IAggregationContext<T, TORM> UserAggrContext { get; }

        public BasicGroupContext( 
            IWriteRepository<TORMGroup> repo,
            IMapper<TGroup, TORMGroup> mapper,
            IDomainMapper domainMapper,
            IAggregationContextProxy<T, TORM> userAggrContext
            ) : base(repo, mapper, domainMapper) {
            this.UserAggrContext = userAggrContext;
        }

        protected abstract Action<IEnumerable<T>> Setter(in TGroup group);
        protected abstract IEnumerable<T>? Collection(in TGroup group);

        protected override void PullMidware(in TGroup group, in TORMGroup obj, 
            bool ignoreCollection = false) {

            if (!ignoreCollection) {
                obj.UnleashSomeTo(Setter(group), UserAggrContext);
            }
        }

        protected override void PushMidware(in TGroup group, in TORMGroup obj) {
            obj.GrabSomeFrom(Collection(group), UserAggrContext);
        }
    }
}