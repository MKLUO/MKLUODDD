using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MKLUODDD.DAL {

    public interface IBaseRepository<TD> where TD : class { 

        // IQueryable<TD> QueryWithCriteria(
        //     in Expression<Func<TD, bool>>? criteria = null, 
        //     Context.UpdLockType updLock = Context.UpdLockType.None);

        IQueryable<TD> QueryWithCriteria(
            in Expression<Func<TD, bool>>? criteria = null);

        IQueryable<TD> QueryWithCriteriaForUpgrade(
            in Expression<Func<TD, bool>>? criteria = null);

        IQueryable<TD> QueryWithCriteriaForUpgradeNoWait(
            in Expression<Func<TD, bool>>? criteria = null);

        // IEnumerable<TD> EnumerateWithCriteria(
        //     in Expression<Func<TD, bool>>? criteria = null, 
        //     bool forUpdate = false);

        // int CountWithCriteria(in Expression<Func<TD, bool>>? criteria = null);

        TD? GetById(int id);
    }

    public interface IRepositoryHandle {
        void ClearPushedCache();
        void Commit();
        void Reset();
    }

    public interface IReadRepository<TD> : IBaseRepository<TD> where TD : class {
        
        void Subscribe(in Context.ISubscriberContext<TD> context);
    }

    public interface IWriteRepository<TD> : IBaseRepository<TD> where TD : class {
        void Add(TD newRecord);

        void Hook(in Context.IHookerContext<TD> context);

        void Update(TD obj);
    }
}
