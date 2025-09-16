namespace lv360_training.Application.Interfaces;

using lv360_training.Domain;

public interface IDbService
{
    IQueryable<User> Users { get; }
    Task AddUserAsync(User user);
    Task<int> SaveChangesAsync();
}
