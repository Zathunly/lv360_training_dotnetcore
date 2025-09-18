using lv360_training.Domain;

namespace lv360_training.Application.Interfaces.Repositories.Core;

public interface ISessionRepository
{
    Task AddAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}
