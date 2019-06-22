
using System.Collections.Generic;

namespace MKLUODDD.Model.ORM
{

    public interface IOwnsSome<T> where T : class {
        ISet<T> Get(T _);
        void Set(IEnumerable<T> ts);
    }

    public interface IOwnsOne<T> where T : class {
        T? Get(T _);
        void Set(T? t);
    }   
}