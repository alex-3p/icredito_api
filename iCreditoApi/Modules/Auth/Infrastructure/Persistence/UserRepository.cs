using iCreditoApi.Modules.Auth.Domain.Entities;
using iCreditoApi.Modules.Auth.Domain.Repositories;
using iCreditoApi.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace iCreditoApi.Modules.Auth.Infrastructure.Persistence;

/// <summary>
/// Implementación del repositorio de usuarios
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var normalizedUsername = username.ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == normalizedUsername, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
    {
        var normalizedUsername = username.ToLowerInvariant();
        return await _context.Users
            .AnyAsync(u => u.Username == normalizedUsername, ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.ToLowerInvariant();
        return await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}
