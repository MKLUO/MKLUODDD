using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MKLUODDD.Context
{
    using DAL;
    using Mapper;

    public class BasicContext<T, TORM> : 
        BaseContext<T, TORM>, 
        IContext<T>,
        IAggregationContext<T, TORM>,
        IPersistContext<T>,
        IHookerContext<TORM>
        where T : class
        where TORM : class, new()
    {

        protected new IWriteRepository<TORM> Repo { get; }
        protected new IMapper<T, TORM> Mapper { get; }
        protected HashSet<T> PushedEnts { get; } = new HashSet<T> { };

        public BasicContext(
            IWriteRepository<TORM> repo,
            IMapper<T, TORM> mapper,
            IDomainMapper domainMapper) : base(repo, mapper, domainMapper) {

            Repo = repo;
            repo.Hook(this); 

            Mapper = mapper;
        }

        public override IList<T> Query(
            Expression<Func<T, bool>> ? criteriaOnT = null, 
            UpdLockType updLock = UpdLockType.None) {
                
            ClearPushedCache();
            Commit();
            ClearPushedCache();

            return base.Query(criteriaOnT, updLock);
        }
        
        #region IAggregationContext<T, TD>

        public override TORM? Push(in T entity) {

            if (!Store.ContainsKey(entity)) {
                if (Add(entity)) return Store[entity];
                else return null;
            }

            if (PushedEnts.Contains(entity))
                return Store[entity];
            PushedEnts.Add(entity);

            var obj = Mapper.Patch(Store[entity], entity);
            PushMidware(entity, obj);

            Repo.Update(Store[entity]);

            return obj;
        }
        
        #endregion

        #region IWriteContext<T>
        public bool AddValidation(in T newEntity) => CheckInvalidEntries(new HashSet<T> { newEntity }).Count == 0;

        public virtual HashSet<T> CheckInvalidEntries(in IEnumerable<T> newEntities) => new HashSet<T>{};

        public bool Add(in T newEntity) => Add(new HashSet<T> { newEntity }, false);

        public bool Add(in ICollection<T> newEntities, bool ignoreFailedEntities = false) {

            #region check
            var actualNewEntities = newEntities.Where(
                ent => !Store.ContainsKey(ent)
            );

            if (!actualNewEntities.Any())
                return false;

            var invalidEntries = CheckInvalidEntries(actualNewEntities);
            
            if (!ignoreFailedEntities && invalidEntries.Count > 0)
                return false;
            #endregion 

            foreach (var newEntity in actualNewEntities.Except(invalidEntries)) {
                if (PushedEnts.Contains(newEntity))
                    return true;
                PushedEnts.Add(newEntity);

                var obj = Mapper.Materialize(newEntity);
                Attach(newEntity, obj);
                PushMidware(newEntity, obj);

                Repo.Add(obj);
            }

            return true;
        }

        public void Commit(T entity) {
            PushedEnts.Remove(entity);
            Push(entity);
        }
        #endregion

        #region IHookerContext<TD> 
        public void ClearPushedCache() => PushedEnts.Clear();

        public void Commit() {
            foreach (var ent in Store.Keys.ToList())
                Push(ent);            
        }

        public override void Reset() {
            ClearPushedCache();
            ClearStore();
        }
        #endregion
    }
}