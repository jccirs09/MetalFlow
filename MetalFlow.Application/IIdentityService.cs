using System.Security.Claims;
using System.Threading.Tasks;

namespace MetalFlow.Application;

public interface IIdentityService
{
    Task<ExternalLoginChallengeDto> ChallengeExternalLoginAsync(string provider, string returnUrl);
    Task SignOutAsync();
    Task<string?> GetPasskeyCreationOptionsAsync(ClaimsPrincipal principal);
    Task<string> GetPasskeyRequestOptionsAsync(string? username);
    Task<ExternalLoginChallengeDto> LinkExternalLoginAsync(ClaimsPrincipal principal, string provider);
}