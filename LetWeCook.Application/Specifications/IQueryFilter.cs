namespace LetWeCook.Application.Specifications
{
    public interface IQueryFilter<TEntity>
    {
        IQueryable<TEntity> Apply(IQueryable<TEntity> query);
    }
}