using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;

namespace MKLUODDD.DAL {
    
    using Context;
 
    public class Repository<TD> : IWriteRepository<TD>, IReadRepository<TD>, IRepositoryHandle where TD : class {

        ISession Session { get; }
        IRegistableContextHandle Handle { get; }
        IHookerContext<TD> ? HookedContext { get; set; } = null;
        HashSet<ISubscriberContext<TD>> SubscribedContexts { get; } = new HashSet<ISubscriberContext<TD>>();

        public Repository(ISession session, IRegistableContextHandle handle) {
            this.Session = session;
            this.Handle = handle;
        }

        public void ClearPushedCache() => HookedContext?.ClearPushedCache();

        public void Commit() => HookedContext?.Commit();

        public void Reset() {
            HookedContext?.Reset();
            foreach (var context in SubscribedContexts)
                context.Reset();
        }
        public IQueryable<TD> QueryWithCriteria(
            in Expression<Func<TD, bool>> ? criteria = null) {

            // TODO: MAX
            // if (CountWithCriteria(criteria) > max)

            IQueryable<TD> query = Session.Query<TD>();

            if (criteria != null)
                query = query.Where(criteria);

            return query;
        }

        public IQueryable<TD> QueryWithCriteriaForUpgrade(
            in Expression<Func<TD, bool>> ? criteria = null)  =>
            QueryWithCriteria(criteria).WithLock(LockMode.Upgrade);

        public IQueryable<TD> QueryWithCriteriaForUpgradeNoWait(
            in Expression<Func<TD, bool>> ? criteria = null)  =>
            QueryWithCriteria(criteria).WithLock(LockMode.UpgradeNoWait);

        public int CountWithCriteria(in Expression<Func<TD, bool>> ? criteria = null) {
            return Session.Query<TD>().Where(criteria).Count();
        }

        public TD? GetById(int id) {
            return Session.Get<TD>(id);
        }

        public void Add(TD newRecord) {
            Session.Save(newRecord);
        }

        // FIXME: There might be LOOP if some Contexts aggregate themselves!
        // (Self-association)

        public void Update(TD obj) {
            foreach (var context in SubscribedContexts)
                context.Update(obj);
        }

        public void Subscribe(in ISubscriberContext<TD> context) {
            SubscribedContexts.Add(context);
            Handle.Register(this);
        }

        public void Hook(in IHookerContext<TD> context) {
            if ((HookedContext != null) && (HookedContext != context))
                throw new MultipleHookerException();
            HookedContext = context;
            Handle.Register(this);
        }

        [System.Serializable]
        public class MultipleHookerException : System.Exception {
            public MultipleHookerException() : this("Only one Context can be hooked to a Repository!") { }
            public MultipleHookerException(string message) : base(message) { }
            public MultipleHookerException(string message, System.Exception inner) : base(message, inner) { }
            protected MultipleHookerException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}