namespace HeadHunter.Models;

public class Module
{
    public int Id { get; set; }
    public DateTime? StartedWorking { get; set; }
    public DateTime? EndedWorking { get; set; }
    public string OrganizationName { get; set; }
    public string Role { get; set; }
    public string Responsibilities { get; set; }
    public string Identity { get; set; }
}