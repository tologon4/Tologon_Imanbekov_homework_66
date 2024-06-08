using Microsoft.AspNetCore.Identity;

namespace HeadHunter.Models;

public class User : IdentityUser<int>
{
    public string Role { get; set; }
    public string Avatar { get; set; }
    public ICollection<Resume>? Resumes { get; set; }
    public ICollection<Vacancy>? Vacancies { get; set; }
    public ICollection<Chat>? Chats { get; set; }
}