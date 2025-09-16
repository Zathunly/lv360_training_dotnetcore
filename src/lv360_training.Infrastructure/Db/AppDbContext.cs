using Microsoft.EntityFrameworkCore;
using lv360_training.Domain;

namespace lv360_training.Infrastructure.Db;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}
