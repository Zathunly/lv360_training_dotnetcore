using lv360_training.Domain.Entities;

namespace lv360_training.Domain.Interfaces.Repositories.Core;

public interface ISessionRepository
{
    Task AddAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
}
