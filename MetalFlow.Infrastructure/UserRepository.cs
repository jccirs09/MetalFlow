using MetalFlow.Application;
using MetalFlow.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MetalFlow.Infrastructure;

public class UserRepository(IApplicationDbContext context) : IUserRepository
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ApplicationUser?> FindByIdAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }
}