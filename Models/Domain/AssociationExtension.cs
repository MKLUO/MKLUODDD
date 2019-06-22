using System.Collections.Generic;

namespace ShiftArr.Model.Domain
{
    public static class AssociationExtension {
        public static void RemoveFromGroup<T>(this T ent, ISet<T>? group) {
            if (group?.Contains(ent) ?? false) 
                group?.Remove(ent);
        }
    }
}