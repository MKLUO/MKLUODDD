namespace ShiftArr.Model.Domain
{
    public class DomainRef<T> where T : class {
        
        T? Item { get; set; } = null;

        public DomainRef(T? item) =>
            Item = item;

        public T? Content => Item;

        public void Set(T item) => this.Item = item;
    }
}