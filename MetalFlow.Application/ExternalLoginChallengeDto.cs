using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace MetalFlow.Application;

public class ExternalLoginChallengeDto
{
    public required string Provider { get; set; }
    public required AuthenticationProperties Properties { get; set; }
}