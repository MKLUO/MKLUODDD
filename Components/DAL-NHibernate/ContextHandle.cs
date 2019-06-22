using System.Collections.Generic;
using NHibernate;

namespace MKLUODDD.Context {
    using Utils;
    using DAL;

    public interface IRegistableContextHandle {
        void Register(IRepositoryHandle repo);
    }

    public class ContextHandle : IContextHandle, IRegistableContextHandle {

        ISession Session { get; }
        IAppLogger? Logger { get; } = null;

        HashSet<IRepositoryHandle> Repositories { get; } = new HashSet<IRepositoryHandle>();

        public ContextHandle(ISession session) {
            this.Session = session;
        }

        public ContextHandle(ISession session, IAppLogger logger) {
            this.Session = session;
            this.Logger = logger;
        }

        public void Register(IRepositoryHandle repo) {
            Repositories.Add(repo);
        }

        public IDomainTransaction BeginDomainTransaction() {
            DomainTransaction transac;
            if ((Session.Transaction != null) && (Session.Transaction?.IsActive ?? false)) {
                transac = DomainTransaction.BeginWithExistingTransaction(
                    Session, Repositories, Session.Transaction);
            } else {
                transac = DomainTransaction.BeginWithNewTransaction(
                    Session, Repositories);            
            }

            if (Logger != null)
                return transac.WithLogger(Logger);
            else return transac;
        }

        [System.Serializable]
        public class StrayTransactionException : System.Exception {
            public StrayTransactionException() : this("NHibernate transaction is distached from previous DomainTransaction!") { }
            public StrayTransactionException(string message) : base(message) { }
            public StrayTransactionException(string message, System.Exception inner) : base(message, inner) { }
            protected StrayTransactionException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }

    public sealed class DomainTransaction : IDomainTransaction {

        #region IDisposable
        private bool disposedValue = false;

        void Dispose(bool disposing) {
            
            if (!disposedValue) {
                if (disposing && OwnThisTransaction) {
                    if (!Transaction.WasCommitted)
                        Rollback();
                    Transaction.Dispose();

                    Logger?.Log($"Transaction owner [{this.GetHashCode()}] disposed.");

                } else 
                    Logger?.Log($"Transaction hooker [{this.GetHashCode()}] disposed.");

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        #endregion

        ITransaction Transaction { get; }

        IEnumerable<IRepositoryHandle> Repositories { get; }

        bool OwnThisTransaction { get; }
        IAppLogger? Logger { get; set; } = null;

        DomainTransaction(
            ISession session, 
            IEnumerable<IRepositoryHandle> repositories,
            ITransaction? hookExistingTransaction = null) {

            this.Transaction = hookExistingTransaction ?? session.BeginTransaction();
            this.OwnThisTransaction = hookExistingTransaction == null;
            this.Repositories = repositories;
        }

        public DomainTransaction WithLogger(IAppLogger logger) {
            this.Logger = logger;
            if (OwnThisTransaction)
                Logger.Log($"New DomainTransaction [{this.GetHashCode()}] created.");
            else
                Logger.Log($"New DomainTransaction [{this.GetHashCode()}] hooked.");

            return this;
        }

        public static DomainTransaction BeginWithNewTransaction(
            ISession session, 
            IEnumerable<IRepositoryHandle> repositories) => 
            new DomainTransaction(session, repositories);

        public static DomainTransaction BeginWithExistingTransaction(
            ISession session, 
            IEnumerable<IRepositoryHandle> repositories,
            ITransaction? existingTransaction) => 
            new DomainTransaction(session, repositories, existingTransaction);

        public void Commit() {

            foreach (var repo in Repositories)
                repo.ClearPushedCache();

            foreach (var repo in Repositories)
                repo.Commit();

            if (OwnThisTransaction) {
                foreach (var repo in Repositories)
                    repo.Reset();

                if (Transaction.IsActive)
                    Transaction.Commit();
            }
        }

        public void Rollback() {
            if (Transaction.IsActive)
                Transaction.Rollback();

            foreach (var repo in Repositories)
                repo.Reset();
        }
    }
}