using MetalFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace MetalFlow.Application;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}