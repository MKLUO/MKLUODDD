namespace MKLUODDD.Model.Domain {

    public interface IInfoMapper<T, TI> {
        TI GetInfoOf(T entity);
    }
}