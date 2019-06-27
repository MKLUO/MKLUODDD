using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace MKLUODDD.Mapper
{
    using MKLUODDD.Model.Domain;
    using MKLUODDD.Model.ORM;
    using Model.Persist;

    public abstract class ReadonlyBaseMapper<T, TD> : IReadMapper<T, TD> where TD : class, IPersist, new() {

        public abstract T Compose(in TD data);

        // protected abstract Dictionary<string, string> MapExprDictionary { get; }

        protected abstract string MapExprKey(string key);

        public virtual Expression MapExpr(in ParameterExpression td, in MemberInfo key) {
            return Expression.MakeMemberAccess(
                td, typeof(TD).GetMember(MapExprKey(key.Name)) [0]
            );
        }

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