namespace chtt.Service
{
    public interface IJoinEntity<TEntity>
    {
        TEntity Navigation { get; set; }
    }
}
