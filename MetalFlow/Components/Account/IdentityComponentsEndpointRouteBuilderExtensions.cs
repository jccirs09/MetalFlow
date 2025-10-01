using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MetalFlow.Application;
using MetalFlow.Components.Account.Pages;
using MetalFlow.Components.Account.Pages.Manage;
using MetalFlow.Domain;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/PerformExternalLogin", async (
            [FromServices] IIdentityService identityService,
            [FromForm] string provider,
            [FromForm] string returnUrl) =>
        {
            var result = await identityService.ChallengeExternalLoginAsync(provider, returnUrl);
            return TypedResults.Challenge(result.Properties, [result.Provider]);
        });

        accountGroup.MapPost("/Logout", async (
            [FromServices] IIdentityService identityService,
            [FromForm] string returnUrl) =>
        {
            await identityService.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        accountGroup.MapPost("/PasskeyCreationOptions", async (
            HttpContext context,
            [FromServices] IIdentityService identityService,
            [FromServices] IAntiforgery antiforgery) =>
        {
            await antiforgery.ValidateRequestAsync(context);
            var optionsJson = await identityService.GetPasskeyCreationOptionsAsync(context.User);
            if (optionsJson is null)
            {
                return Results.NotFound("User not found.");
            }
            return TypedResults.Content(optionsJson, contentType: "application/json");
        });

        accountGroup.MapPost("/PasskeyRequestOptions", async (
            HttpContext context,
            [FromServices] IIdentityService identityService,
            [FromServices] IAntiforgery antiforgery,
            [FromQuery] string? username) =>
        {
            await antiforgery.ValidateRequestAsync(context);
            var optionsJson = await identityService.GetPasskeyRequestOptionsAsync(username);
            return TypedResults.Content(optionsJson, contentType: "application/json");
        });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        manageGroup.MapPost("/LinkExternalLogin", async (
            HttpContext context,
            [FromServices] IIdentityService identityService,
            [FromForm] string provider) =>
        {
            var result = await identityService.LinkExternalLoginAsync(context.User, provider);
            return TypedResults.Challenge(result.Properties, [result.Provider]);
        });

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] IPersonalDataService personalDataService) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            }

            var userId = await userManager.GetUserIdAsync(user);
            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            var personalData = await personalDataService.GetPersonalDataAsync(userId);
            var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
        });

        return accountGroup;
    }
}
