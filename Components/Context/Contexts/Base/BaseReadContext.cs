using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MKLUODDD.Context
{

    using DAL;
    using Mapper;

    public abstract class BaseReadContext<T, TD> : 
        IReadContext<T>,
        IAggregationContext<T, TD>
        where T : class
        where TD : class, new() 
    {
        protected IBaseRepository<TD> Repo { get; }
        protected IReadMapper<T, TD> Mapper { get; }
        protected IDomainMapper DomainMapper { get; }
        protected Dictionary<T, TD> Store { get; } = new Dictionary<T, TD> { };
        protected Dictionary<TD, T> StoreLookUp { get; } = new Dictionary<TD, T> { };

        protected BaseReadContext(
            IBaseRepository<TD> repo,
            IReadMapper<T, TD> mapper,
            IDomainMapper domainMapper) {

            Repo = repo;
            Mapper = mapper;
            DomainMapper = domainMapper;
        }

        protected void Attach(in T entity, in TD obj) {
            if (StoreLookUp.ContainsKey(obj)) {
                Store.Remove(StoreLookUp[obj]);
                StoreLookUp.Remove(obj);
            }
            Store[entity] = obj;
            StoreLookUp[obj] = entity;
        }

        protected void ClearStore() {
            Store.Clear();
            StoreLookUp.Clear();
        }

        protected Expression<Func<TD, bool>> TrasformExp(in Expression<Func<T, bool>> criteriaOnT) {
            ParameterExpression td = Expression.Parameter(typeof(TD));

            var visitor = new DomainToOrmVisitor<T, TD>(Mapper, DomainMapper, td);
            var transformedBody = visitor.Visit(criteriaOnT.Body);

            return Expression.Lambda<Func<TD, bool>>(
                transformedBody,
                new List<ParameterExpression> { td }
            );
        }

        IQueryable<TD> GetQuery(
            Expression<Func<T, bool>> ? criteriaOnT = null, 
            UpdLockType updLock = UpdLockType.None) {
                
            var criteriaOnTD = (criteriaOnT != null) ? 
                TrasformExp(criteriaOnT) : null;

            return updLock switch {
                UpdLockType.Upgrade =>
                    Repo.QueryWithCriteriaForUpgrade(criteriaOnTD),
                UpdLockType.UpgradeNoWait =>
                    Repo.QueryWithCriteriaForUpgradeNoWait(criteriaOnTD),
                UpdLockType.None =>
                    Repo.QueryWithCriteria(criteriaOnTD),
                _ => 
                    Repo.QueryWithCriteria(criteriaOnTD)
            };
        }

        public virtual IList<T> Query(
            Expression<Func<T, bool>> ? criteriaOnT = null,
            UpdLockType updLock = UpdLockType.None, 
            bool ignoreCollection = false) {

            if (Count(criteriaOnT) > 1000) {
                // TODO: TooBigQueryException
                // throw new TooBigQueryException();
            }

            HashSet<T> entities = new HashSet<T> { };

            var query = GetQuery(criteriaOnT, updLock);

            foreach (var obj in query) {
                entities.Add(Pull(obj, ignoreCollection));
            }
            return entities.ToList();
        }

        public virtual IEnumerable<T> Enumerate(
            Expression<Func<T, bool>>? criteriaOnT = null,
            UpdLockType updLock = UpdLockType.None, 
            bool ignoreCollection = false,
            int batchSize = 0) {

            int amount = Count(criteriaOnT);

            var query = GetQuery(criteriaOnT, updLock);

            int size = (batchSize > 0) ? batchSize : 100;
            int start = 0;
            while (start < amount) {
                IQueryable<TD> batch = query.Skip(start).Take(size);

                foreach (var obj in batch)
                    yield return Pull(obj, ignoreCollection);
                
                start += size;
            }
        }

        public int Count(in Expression<Func<T, bool>>? criteriaOnT = null, 
            UpdLockType updLock = UpdLockType.None) => 
            GetQuery(criteriaOnT, updLock).Count();

        public virtual void Reset() => ClearStore();
        
        public virtual void DrawCollection(in T entity) {}
        
        public virtual T Pull(in TD obj, bool ignoreCollection = false) {
            if (StoreLookUp.ContainsKey(obj))
                return StoreLookUp[obj];

            var entity = Mapper.Compose(obj);
            Attach(entity, obj);

            return entity;
        }

        public virtual TD? Push(in T entity) { return null; }
    }
}