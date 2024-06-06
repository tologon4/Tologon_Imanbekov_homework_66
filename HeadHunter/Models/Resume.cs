using System.ComponentModel.DataAnnotations;

namespace HeadHunter.Models;

public class Resume
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Заполните поле Title!")]
    public string Title { get; set; }
    public DateTime? EditedTime { get; set; }
    public DateTime? CreatedTime { get; set; }
    public string? UserAvatar { get; set; }
    public bool? Published { get; set; }
    [Required(ErrorMessage = "Заполните поле Category!")]
    public string Category { get; set; }
    [Required(ErrorMessage = "Заполните поле Фамилие!")]
    public string UserLastName { get; set; }
    [Required(ErrorMessage = "Заполните поле Имя!")]
    public string UserFirstName { get; set; }
    [Required(ErrorMessage = "Заполните поле ожидаемое заработная плата!")]
    [Range(0, int.MaxValue, ErrorMessage = "Значение не может быть меньше 0")]
    public int ExpectedSalary { get; set; }
    [Required(ErrorMessage = "Заполните поле Telegram!")]
    public string Telegram { get; set; }
    [Required(ErrorMessage = "Заполните поле Email!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Заполните поле номер телефона!")]
    public string PhoneNumber { get; set; }
    
    public string? Facebook { get; set; }
    public string? LinkedIn { get; set; }
    public ICollection<Module>? Modules { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    
}