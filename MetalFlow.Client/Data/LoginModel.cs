using System.ComponentModel.DataAnnotations;

namespace MetalFlow.Client.Data;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }
}