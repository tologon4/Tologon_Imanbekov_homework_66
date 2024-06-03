using Microsoft.AspNetCore.Identity;

namespace HeadHunter.Models;

public class User : IdentityUser<int>
{
    public string Role { get; set; }
    public string Avatar { get; set; }
}