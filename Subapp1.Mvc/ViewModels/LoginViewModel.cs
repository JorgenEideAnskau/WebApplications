using System.ComponentModel.DataAnnotations;

namespace Subapp1.Mvc.ViewModels;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
