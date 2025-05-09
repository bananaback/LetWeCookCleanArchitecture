using LetWeCook.Application.Enums;

namespace LetWeCook.Application.Specifications;

public interface ISortFilter<TEntity> where TEntity : class
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query);
}