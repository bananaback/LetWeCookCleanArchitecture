using LetWeCook.Application.Interfaces;
using LetWeCook.Domain.Entities;
using LetWeCook.Infrastructure.Persistence;

namespace LetWeCook.Infrastructure.Repositories;

public class SuggestionFeedbackRepository : Repository<SuggestionFeedback>, ISuggestionFeedbackRepository
{
    public SuggestionFeedbackRepository(LetWeCookDbContext context) : base(context)
    {
    }


}