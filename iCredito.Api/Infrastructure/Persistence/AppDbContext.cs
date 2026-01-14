using Microsoft.EntityFrameworkCore;
using iCredito.Api.Domain.Entities;

namespace iCredito.Api.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}
