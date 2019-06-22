namespace MKLUODDD.Context
{
    public interface IAggregationContextProxy<T, TD> : IAggregationContext<T, TD> where TD : class {}
}