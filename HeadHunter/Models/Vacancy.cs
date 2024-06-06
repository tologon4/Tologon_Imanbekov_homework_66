using System.ComponentModel.DataAnnotations;

namespace HeadHunter.Models;

public class Vacancy
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    [Required(ErrorMessage = "Заполните поле!")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Заполните поле!")]
    public int Salary { get; set; }
    [Required(ErrorMessage = "Заполните поле!")]
    public string Demands { get; set; }
    [Required(ErrorMessage = "Заполните поле!")]
    [Range(0, int.MaxValue)]
    public int ExperienceFrom { get; set; }
    [Required(ErrorMessage = "Заполните поле!")]
    [Range(0, int.MaxValue)]
    public int ExperienceTo { get; set; }
    [Required(ErrorMessage = "Заполните поле!")]
    public string Category { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? EditedTime { get; set; }
    public bool? Published { get; set; }
}