namespace LetWeCook.Application.Specifications;

public interface ISpecification<TEntity> where TEntity : class
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query);
}