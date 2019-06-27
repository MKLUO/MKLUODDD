using System.Collections.Generic;

namespace MKLUODDD.Model.ORM
{
    public interface INavSome<T> where T : class, new() {
        // ISet<T> Get(T _);
        // virtual void Set(IEnumerable<T> ts) => Get(new T()).ClearAndUnionWith(ts);
        // virtual void Add(T t) => Get(new T()).Add(t);
        // virtual void Remove(T t) => Get(new T()).Remove(t);

        CollectionNavigator<T> NavSome(T _);
    }

    public interface INav<T> where T : class, new() {
        // T? Get(T _);
        // void Set(T? t);

        Navigator<T> Nav(T _);
    }    

    // public interface IBelongsTo<T> where T : class {
    //     T? Find(T _);
    //     void MoveTo(T? t);
    // }
}