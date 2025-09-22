
using lv360_training.Infrastructure.Persistence;
using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Repositories.Core;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;

    public SessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Session session) =>
        await _context.Sessions.AddAsync(session);

    public async Task<Session?> GetByIdAsync(Guid id) =>
        await _context.Sessions
            .Include(s => s.User) 
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task DeleteAsync(Guid id)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id);
        if (session != null)
            _context.Sessions.Remove(session);
    }
}
