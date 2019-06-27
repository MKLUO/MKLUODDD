#nullable enable

using System;
using System.Collections.Generic;

namespace MKLUODDD.Model.ORM
{
    public static class NavigatorExtension {
        public static void ClearAndUnionWith<T>(this ISet<T> set, IEnumerable<T> ts) {
            set.Clear();
            set.UnionWith(ts);
        }
    }

    public class CollectionNavigator<T> {
        ISet<T> Objs { get; }

        public CollectionNavigator(ISet<T> objs) => Objs = objs;

        public ISet<T> Get() => Objs;
        public void Set(IEnumerable<T> ts) => Objs.ClearAndUnionWith(ts);
        public void Add(T t) => Objs.Add(t);
        public void Remove(T t) => Objs.Remove(t);
    }

    public class Navigator<T> where T : class {
        // T? Obj { get; set; }
        Func<T> Getter { get; }
        Action<T?> Setter { get; }

        public Navigator(Func<T> getter, Action<T?> setter) {
            Getter = getter;
            Setter = setter;
        }

        public T? Get() => Getter();
        public void Set(T? t) => Setter(t);
    }
}