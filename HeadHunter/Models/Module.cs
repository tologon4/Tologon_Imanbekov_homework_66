namespace HeadHunter.Models;

public class Module
{
    public int Id { get; set; }
    public int StartedWorkingYear { get; set; }
    public int EndedWorkingYear { get; set; }
    public string OrganizationName { get; set; }
    public string Role { get; set; }
    public string Responsibilities { get; set; }
}