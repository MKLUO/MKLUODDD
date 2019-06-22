using System.Reflection;
using System.Linq.Expressions;

namespace MKLUODDD.Mapper
{
    using MKLUODDD.Model.Domain;
    using MKLUODDD.Model.ORM;
    using Model.Persist;

    public abstract class ReadonlyBaseMapper<T, TD> : IReadMapper<T, TD> where TD : class, IPersist, new() {
        public abstract T Compose(in TD data);
        public abstract Expression MapExpr(in ParameterExpression td, in MemberInfo key);

        // TODO: BAD! not generic!
        public virtual object? GetAggrChild(in TD obj) => null;
        public virtual void SetAggrChild(in TD obj, object? child) { return; }

        public virtual PersistInfo<T> InfoOf(in TD obj) {
            return new PersistInfo<T>(obj.Id, new ByteArray(obj.Version));
        }
    }

    public abstract class BaseMapper<T, TD> : ReadonlyBaseMapper<T, TD>, IMapper<T, TD> where TD : class, IPersist, new() {
        public TD Materialize(in T entity) {
            return Patch(new TD(), entity);
        }
        public abstract TD Patch(in TD obj, in T entity);
    }
}