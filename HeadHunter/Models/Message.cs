namespace HeadHunter.Models;

public class Message
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string? MessageContent { get; set; }
    public int? ResumeId { get; set; }
    public Resume? Resume { get; set; }
    public int? ChatId { get; set; }
    public Chat? Chat { get; set; }
    public DateTime? SentTime { get; set; }
}