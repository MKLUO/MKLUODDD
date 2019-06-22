using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MKLUODDD.Context
{

    using Model.Persist;

    public interface IReadContext<T> where T : class {

        /// <summary>
        /// Queries the context with given criteria.
        /// Caution: Supports only SQL-styled criterias.
        /// </summary>
        /// <param name="criteriaOnT">Criteria.</param>
        /// <returns>Query result.</returns>        
        IList<T> Query(
            Expression<Func<T, bool>>? criteriaOnT = null, 
            UpdLockType updLock = UpdLockType.None);
            
        IEnumerable<T> Enumerate(
            Expression<Func<T, bool>>? criteriaOnT = null, 
            UpdLockType updLock = UpdLockType.None,
            int batchSize = 0);

        void Draw(in T entity);

        int Count(in Expression<Func<T, bool>>? criteriaOnT = null);
    }

    /// <summary>
    /// Collection-like entity context with unit-of-work.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IWriteContext<T> where T : class {

        /// <summary>
        /// Validates whether given entity can be added to the context.
        /// </summary>
        /// <param name="newEntity">New entity.</param>
        /// <returns>The given entity can be added.</returns>
        bool AddValidation(in T newEntity);      
        HashSet<T> CheckInvalidEntries(in IEnumerable<T> newEntities);

        /// <summary>
        /// Add the given new entity to context.
        /// Fails if AddValidation(newEntity) is false.
        /// If entity is already in the context, nothing would be done.
        /// </summary>
        /// <param name="newEntity">New entity.</param>
        /// <returns>Add successed.</returns>
        bool Add(in T newEntity);        
        bool Add(in ICollection<T> newEntities, bool ignoreFailedEntities = false);

        void Commit(T entity);
    }

    public interface IContext<T> : IReadContext<T>, IWriteContext<T> 
        where T : class {}

    /// <summary>
    /// Context utilities which enables transferring persistance info between different sessions.
    /// Useful when entities have to leave Domain layer.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IPersistContext<T> where T : class {

        /// <summary>
        /// Obtains persistance info of given entity.
        /// Returns null if the entity is not present.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Persistance info.</returns>
        PersistInfo<T>? GetPersistInfo(in T entity);

        /// <summary>
        /// Obtains entity of given persistance info.
        /// Returns null if info is invalid or version mismatched (can be bypassed by force). 
        /// </summary>
        /// <param name="info">Persistance info.</param>
        /// <param name="force">Ignore version check.</param>
        /// <returns>Entity.</returns>
        T? GetEntity(in PersistInfo<T> info, bool force = false);
    }

    /// <summary>
    /// Context utilities which enables entity/ORM instances to be aggregated accross multiple contexts.
    /// Useful for aggregation between contexts.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <typeparam name="TD">ORM type.</typeparam>
    public interface IAggregationContext<T, TD> where TD : class {

        /// <summary>
        /// Instantiate an entity from given ORM object.
        /// </summary>
        /// <param name="obj">ORM object.</param>
        /// <param name="halt">Don't fetch its collections.</param>
        /// <returns>Instantiated entity.</returns>
        T Pull(in TD obj, bool halt = false);

        /// <summary>
        /// Get and patch corresponding ORM object of given entity.
        /// If entity is not present, add it to the context.
        /// If add failed, returns null.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Patched ORM object.</returns>
        TD? Push(in T entity);
        
        // /// <summary>
        /// // /// Create transient ORM object of given entity.
        // /// </summary>
        // /// <param name="entity">Entity.</param>
        // /// <returns>Transient ORM object.</returns>
        // TD PushTransient(in T entity);
    }

    public interface IResettableContext {        
        /// <summary>
        /// Abandons persistent entities.
        /// Caution: After calling this method, all the entities acquired would be detached, which no longer correspond to any ORM object.
        /// </summary>
        void Reset();
    }

    public interface ISubscriberContext<TD> : IResettableContext where TD : class {
        void Update(TD obj);
    }

    public interface IHookerContext<TD> : IResettableContext { 

        void ClearPushedCache();

        /// <summary>
        /// Commits persistent entities into ORM.
        /// </summary>
        void Commit();

    }
}