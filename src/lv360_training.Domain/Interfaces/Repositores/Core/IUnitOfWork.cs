namespace lv360_training.Domain.Interfaces.Repositories.Core;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
