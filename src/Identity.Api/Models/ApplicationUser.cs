using Microsoft.AspNetCore.Identity;

namespace NEFORmal.ua.Identity.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string? ProfilePhoto { get; set; }
    public string? RefreshToken { get; set; }
    public int?    Code         { get; set; } 
}