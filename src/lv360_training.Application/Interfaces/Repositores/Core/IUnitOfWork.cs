namespace lv360_training.Application.Interfaces.Repositories.Core;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
