

using System;

namespace MKLUODDD.Context {

    public interface IContextHandle {

        IDomainTransaction BeginDomainTransaction();
        // void Commit();
        // void Rollback();
    }

    public interface IDomainTransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}