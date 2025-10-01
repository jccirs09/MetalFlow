using MetalFlow.Domain;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetalFlow.Application;

public class PersonalDataService(UserManager<ApplicationUser> userManager, IUserRepository userRepository) : IPersonalDataService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Dictionary<string, string>> GetPersonalDataAsync(string userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user is null)
        {
            return [];
        }

        var personalData = new Dictionary<string, string>();
        var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        foreach (var p in personalDataProps)
        {
            personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        }

        var logins = await _userManager.GetLoginsAsync(user);
        foreach (var l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add("Authenticator Key", (await _userManager.GetAuthenticatorKeyAsync(user))!);
        return personalData;
    }
}