using System.Threading.Tasks;

namespace MetalFlow.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password);
        Task LogoutAsync();
    }
}