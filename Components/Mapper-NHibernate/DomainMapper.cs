using System;
using System.Reflection;

namespace MKLUODDD.Mapper {

    using Model.Domain;

    // TODO: Unfinished

    public class DomainMapper : IDomainMapper {

        public MethodInfo MapMethod(object obj, MethodInfo method) {

            switch (obj, method.Name) {

                case (String _, _):
                    return typeof(string).GetMethod(method.Name, new[]{typeof(string)});

                default:
                    throw new Exception();
            }
        }
    }
}