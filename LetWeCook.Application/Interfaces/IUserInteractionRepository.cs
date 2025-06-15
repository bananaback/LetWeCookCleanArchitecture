using LetWeCook.Application.DTOs.UserInteractions;
using LetWeCook.Domain.Entities;

namespace LetWeCook.Application.Interfaces;

public interface IUserInteractionRepository : IRepository<UserInteraction>
{
    Task<List<AggregatedInteractionDto>> GetAggregatedInteractionsAsync();

}