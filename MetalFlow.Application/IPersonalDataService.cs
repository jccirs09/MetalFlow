using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetalFlow.Application;

public interface IPersonalDataService
{
    Task<Dictionary<string, string>> GetPersonalDataAsync(string userId);
}