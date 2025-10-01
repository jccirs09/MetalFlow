using MetalFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MetalFlow.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}