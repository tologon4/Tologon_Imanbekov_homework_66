namespace HeadHunter.Models;

public class Chat
{
    public int Id { get; set; }
    public int? VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }
    public int? ApplicantId { get; set; }
    public User? Applicant { get; set; }
    public ICollection<Message>? Messages { get; set; }
}