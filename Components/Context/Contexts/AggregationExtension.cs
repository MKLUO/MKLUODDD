
using System;
using System.Collections.Generic;
using System.Linq;
using MKLUODDD.Context;

namespace MKLUODDD.Model.ORM
{
    public static class AggregationExtension {
        public static void GrabSomeFrom<TORM, TD, TDORM>(
            this TORM obj, 
            ISet<TD>? maybeChildObjs, 
            IAggregationContext<TD, TDORM> context)  
            where TORM : IOwnsSome<TDORM> 
            where TDORM : class {

            if (maybeChildObjs is ISet<TD> childObjs)
                obj.Set(childObjs
                .Select(t => context.Push(t))
                .Where(t => t is TDORM).Cast<TDORM>());
        }

        public static void UnleashSomeTo<TORM, TD, TDORM>(
            this TORM obj, 
            Action<IEnumerable<TD>> setter, 
            IAggregationContext<TD, TDORM> context)  
            where TORM : IOwnsSome<TDORM> 
            where TDORM : class, new() {

            setter(
                obj.Get(new TDORM())
                .Select(t => context.Pull(t, halt: true)));
        }

        public static void GrabFrom<TORM, TD, TDORM>(
            this TORM obj, 
            TD? maybeChildObj, 
            IAggregationContext<TD, TDORM> context)  
            where TD : class
            where TORM : IOwnsOne<TDORM> 
            where TDORM : class {

            if (maybeChildObj is TD childObj)
                obj.Set(context.Push(childObj));
            else 
                obj.Set(null);
        }

        public static void UnleashTo<TORM, TD, TDORM>(
            this TORM obj, 
            Action<TD?> setter, 
            IAggregationContext<TD, TDORM> context)  
            where TD : class
            where TORM : IOwnsOne<TDORM> 
            where TDORM : class, new() {

            if (obj.Get(new TDORM()) is TDORM tdorm)
                setter(context.Pull(tdorm, halt: true));
            else 
                setter(null);
        }
    }

}