using MetalFlow.Domain;
using System.Threading.Tasks;

namespace MetalFlow.Application;

public interface IUserRepository
{
    // This is a sample method. In a real application, you would add methods
    // for all the required user-related data operations.
    Task<ApplicationUser?> FindByIdAsync(string userId);
}