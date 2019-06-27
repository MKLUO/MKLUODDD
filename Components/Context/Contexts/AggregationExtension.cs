
using System;
using System.Collections.Generic;
using System.Linq;
using MKLUODDD.Context;

namespace MKLUODDD.Model.ORM
{
    public static class AggregationExtension {
        public static void GrabSomeFrom<TORM, TD, TDORM>(
            this TORM obj, 
            IEnumerable<TD>? maybeChildEnts, 
            IAggregationContext<TD, TDORM> context)  
            where TORM : class, INavSome<TDORM>, new()
            where TDORM : class, INav<TORM>, new()  {

            if (maybeChildEnts is IEnumerable<TD> childEnts) {

                var transientChilds = childEnts.Where(t => context.IsTransient(t))
                    .Select(t => (ent: t, obj: context.PushTransient(t))).ToList();
                var persistChildObjs = childEnts.Where(t => !context.IsTransient(t))
                    .Select(t => context.Push(t)).Where(t => t is TDORM).Cast<TDORM>().ToList();

                var childObjs = transientChilds.Select(c => c.obj).Union(persistChildObjs);
                obj.NavSome(new TDORM()).Set(childObjs);

                foreach (var child in transientChilds) {
                    child.obj.Nav(new TORM()).Set(obj);
                    context.AttachTransient(child.ent, child.obj);
                }
                foreach (var childObj in persistChildObjs) {
                    childObj.Nav(new TORM()).Set(obj);
                }
            }
        }


        public static void UnleashSomeTo<TORM, TD, TDORM>(
            this TORM obj, 
            Action<IEnumerable<TD>> setter, 
            IAggregationContext<TD, TDORM> context)  
            where TORM : INavSome<TDORM> 
            where TDORM : class, new() {

            setter(
                obj.NavSome(new TDORM()).Get()
                .Select(t => context.Pull(t, ignoreCollection: true)));
        }

        public static void GrabFrom<TORM, TD, TDORM>(
            this TORM obj, 
            TD? maybeParentEnt, 
            IAggregationContext<TD, TDORM> context)  
            where TD : class
            where TORM : class, INav<TDORM>, new() 
            where TDORM : class, INavSome<TORM>, new() {

            // FIXME: Throw exception if things goes wrong here so transaction can be rolled back. (obj mustn't be stray!)
            if (maybeParentEnt is TD parentEnt) {
                var parentObj = context.Push(parentEnt);
                if (parentObj != obj.Nav(new TDORM()).Get()) {
                    obj.Nav(new TDORM()).Get()?.NavSome(new TORM()).Remove(obj);
                    obj.Nav(new TDORM()).Set(parentObj);
                    parentObj?.NavSome(new TORM())?.Add(obj);
                }
            } else 
                obj.Nav(new TDORM()).Set(null);
        }

        public static void UnleashTo<TORM, TD, TDORM>(
            this TORM obj, 
            Action<TD> setter, 
            IAggregationContext<TD, TDORM> context)  
            where TD : class
            where TORM : INav<TDORM> 
            where TDORM : class, new() {

            if (obj.Nav(new TDORM()).Get() is TDORM tdorm)
                setter(context.Pull(tdorm, ignoreCollection: true));
        }
    }

}