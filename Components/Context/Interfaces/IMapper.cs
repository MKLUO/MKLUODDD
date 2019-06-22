using System.Linq.Expressions;
using System.Reflection;

namespace MKLUODDD.Mapper
{
    using Model.Persist;

    public interface IDomainMapper {
        MethodInfo MapMethod(object obj, MethodInfo method);
    }

    public interface IReadMapper<T, TD> {
        
        T Compose(in TD data);
        
        Expression MapExpr(in ParameterExpression td, in MemberInfo key);
        
        /// <summary>
        /// Obtains persistance info from given ORM object.
        /// Usually called by Applicat
        /// </summary>
        /// <param name="obj">ORM object.</param>
        /// <returns>Persistance info.</returns>
        PersistInfo<T> InfoOf(in TD obj);
    }

    public interface IWriteMapper<T, TD> where TD : new() {

        /// <summary>
        /// Flatten the given entity into a (detached) new ORM object.
        /// </summary>
        /// <param name="entity">Entity to be flattened.</param>
        /// <returns>New ORM object.</returns>
        TD Materialize(in T entity);

        /// <summary>
        /// Patch the content of given ORM object with given entity.
        /// </summary>
        /// <param name="obj">ORM object to be patched.</param>
        /// <param name="entity">Entity.</param>
        /// <returns>Patched ORM object.</returns>
        TD Patch(in TD obj, in T entity);

    }

    // public interface IAggregationMapper<T, TD> where TD : new() { 
    //     object? GetAggrChild(in TD obj);
    //     void SetAggrChild(in TD obj, object? child);
    // }

    public interface IMapper<T, TD> : 
        IWriteMapper<T, TD>, 
        IReadonlyMapper<T, TD>
        where TD : new() { }

    // public interface IReadonlyMapper<T, TD> : 
    //     IReadMapper<T, TD>,
    //     IAggregationMapper<T, TD>
    //     where TD : new() { }

    public interface IReadonlyMapper<T, TD> : 
        IReadMapper<T, TD>
        where TD : new() { }

    public interface IViewMapper<T, TD> :
        IReadonlyMapper<T, TD>
        where TD : new() {
        T ComposeInplace(in T entity, in TD data);
    }
}