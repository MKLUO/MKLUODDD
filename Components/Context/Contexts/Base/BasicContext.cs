using System.Collections.Generic;
using System.Linq;

namespace MKLUODDD.Context
{
    using DAL;
    using Mapper;

    public class BasicContext<T, TD> : 
        BaseContext<T, TD>, 
        IContext<T>,
        IAggregationContext<T, TD>,
        IPersistContext<T>,
        IHookerContext<TD>
        where T : class
        where TD : class, new()
    {

        protected new IWriteRepository<TD> Repo { get; }
        protected new IMapper<T, TD> Mapper { get; }
        protected HashSet<T> PushedEnts { get; } = new HashSet<T> { };

        public BasicContext(
            IWriteRepository<TD> repo,
            IMapper<T, TD> mapper,
            IDomainMapper domainMapper) : base(repo, mapper, domainMapper) {

            Repo = repo;
            repo.Hook(this); 

            Mapper = mapper;
        }
        
        #region IAggregationContext<T, TD>

        public override TD? Push(in T entity) {
            if (!Store.ContainsKey(entity))
                return null;

            if (PushedEnts.Contains(entity))
                return Store[entity];

            var obj = Mapper.Patch(Store[entity], entity);
            PushedEnts.Add(entity);

            PushMidware(entity, obj);

            Repo.Update(Store[entity]);

            return obj;
        }

        public virtual TD PushTransient(in T entity) {
            var obj = Mapper.Materialize(entity);
            PushMidware(entity, obj);
            return obj;
        }
        #endregion

        #region IWriteContext<T>
        public bool AddValidation(in T newEntity) => CheckInvalidEntries(new HashSet<T> { newEntity }).Count == 0;

        public virtual HashSet<T> CheckInvalidEntries(in IEnumerable<T> newEntities) => new HashSet<T>{};

        public bool Add(in T newEntity) => Add(new HashSet<T> { newEntity }, false);

        public bool Add(in ICollection<T> newEntities, bool ignoreFailedEntities = false) {

            var actualNewEntities = newEntities.Where(
                ent => !Store.ContainsKey(ent)
            );

            if (!actualNewEntities.Any())
                return false;

            var invalidEntries = CheckInvalidEntries(actualNewEntities);
            
            if (!ignoreFailedEntities && invalidEntries.Count > 0)
                return false;

            foreach (var newEntity in actualNewEntities.Except(invalidEntries)) {

                var obj = PushTransient(newEntity);
                Attach(newEntity, obj);
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