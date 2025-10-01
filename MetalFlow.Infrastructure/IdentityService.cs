using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MetalFlow.Application;
using MetalFlow.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;

namespace MetalFlow.Infrastructure;

public class IdentityService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IIdentityService
{
    public async Task<ExternalLoginChallengeDto> ChallengeExternalLoginAsync(string provider, string returnUrl)
    {
        var httpContext = httpContextAccessor.HttpContext!;
        IEnumerable<KeyValuePair<string, StringValues>> query =
        [
            new("ReturnUrl", returnUrl),
            new("Action", "LoginCallback")
        ];

        var redirectUrl = UriHelper.BuildRelative(
            httpContext.Request.PathBase,
            "/Account/ExternalLogin",
            QueryString.Create(query));

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return await Task.FromResult(new ExternalLoginChallengeDto { Provider = provider, Properties = properties });
    }

    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
    }

    public async Task<string?> GetPasskeyCreationOptionsAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return null;
        }

        var userId = await userManager.GetUserIdAsync(user);
        var userName = await userManager.GetUserNameAsync(user) ?? "User";
        var optionsJson = await signInManager.MakePasskeyCreationOptionsAsync(new()
        {
            Id = userId,
            Name = userName,
            DisplayName = userName
        });
        return optionsJson;
    }

    public async Task<string> GetPasskeyRequestOptionsAsync(string? username)
    {
        var user = string.IsNullOrEmpty(username) ? null : await userManager.FindByNameAsync(username);
        var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);
        return optionsJson;
    }

    public async Task<ExternalLoginChallengeDto> LinkExternalLoginAsync(ClaimsPrincipal principal, string provider)
    {
        var httpContext = httpContextAccessor.HttpContext!;
        await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var redirectUrl = UriHelper.BuildRelative(
            httpContext.Request.PathBase,
            "/Account/Manage/ExternalLogins",
            QueryString.Create("Action", "LinkLoginCallback"));

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(principal));
        return await Task.FromResult(new ExternalLoginChallengeDto { Provider = provider, Properties = properties });
    }
}