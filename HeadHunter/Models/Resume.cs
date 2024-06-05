namespace HeadHunter.Models;

public class Resume
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateTime? EditedTime { get; set; }
    public DateTime? CreatedTime { get; set; }
    public List<Module>? Modules { get; set; }
    public bool? Published { get; set; }
    public string? Category { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFirstName { get; set; }
    public int? ExpectedSalary { get; set; }
    public string Telegram { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? Facebook { get; set; }
    public string? LinkedIn { get; set; }
    
}