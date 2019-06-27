using System;
using System.Collections.Generic;
using System.Linq;

namespace ShiftArr.Model.Domain {

    public class DomainCollection<T> {

        HashSet<T> ? Items { get; set; } = null;
        Func<T, bool> ValidateFunc { get; }

        public DomainCollection(
            IEnumerable<T> ? items,
            Func<T, bool> ? validateFunc = null) {
            this.ValidateFunc = validateFunc ?? (t => true);
            if (items != null)
                this.Items = new HashSet<T>(items);
        }

        public DomainCollection(
            ISet<T> ? items,
            Func<T, bool> ? validateFunc = null) : this(items?.AsEnumerable(), validateFunc) { }

        public static DomainCollection<T> New(Func<T, bool> ? validateFunc = null) =>
        new DomainCollection<T>(new HashSet<T> { }, validateFunc);

        public IEnumerable<T> ? Content =>
        Items?.AsEnumerable();

        public IEnumerable<T> Enumeration =>
        Content ?? new List<T>{};


        public bool IsLoaded() => Items != null;

        public void Set(IEnumerable<T> items) =>
        this.Items = new HashSet<T>(items);

        public bool Validate(T newItem) => ValidateFunc(newItem);

        public void Add(T newItem) {
            if (!Validate(newItem)) throw new Exception();
            if (Items == null) throw new Exception();
            Items.Add(newItem);
        }

        public void Remove(T newItem) {
            if (Items == null) throw new Exception();
            Items?.Remove(newItem);
        }
    }
}