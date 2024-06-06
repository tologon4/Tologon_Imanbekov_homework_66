using HeadHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeadHunter.Controllers;

public class VacancyController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public VacancyController(HeadHunterDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = new List<string>()
        {
            "Программист", "Бухгалтер", "Экономист", "Слесарь", "Учитель", "Доктор", "Инженер", "Водитель", "Охраник", "Садовник"
        };
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        ViewBag.Categories = new List<string>()
        {
            "Программист", "Бухгалтер", "Экономист", "Слесарь", "Учитель", "Доктор", "Инженер", "Водитель", "Охраник", "Садовник"
        };
        Vacancy vacancy = await _db.Vacancies.FirstOrDefaultAsync(v => v.Id == id);
        return View(vacancy);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(int? vacancyId, string? key, string? value)
    {
        User user = await _userManager.GetUserAsync(User);
        Vacancy vacancy = await _db.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyId);
        bool result;
        switch (key)
        {
            case "name":
                vacancy.Name = value;
                result = true;
                break;
            case "salary":
                vacancy.Salary = int.Parse(value);
                result = true;
                break;
            case "demands":
                vacancy.Demands = value;
                result = true;
                break;
            case "exFrom":
                vacancy.ExperienceFrom = int.Parse(value);
                result = true;
                break;
            case "exTo":
                vacancy.ExperienceTo = int.Parse(value);
                result = true;
                break;
            case "category":
                vacancy.Category = value;
                result = true;
                break;
            default:
                result = false;
                break;
        }
        vacancy.EditedTime = DateTime.UtcNow;
        _db.Vacancies.Update(vacancy);
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Vacancy? model)
    {
        ViewBag.Categories = new List<string>()
        {
            "Программист", "Бухгалтер", "Экономист", "Слесарь", "Учитель", "Доктор", "Инженер", "Водитель", "Охраник", "Садовник"
        };
        User? user = await _userManager.GetUserAsync(User);
        if (ModelState.IsValid && user != null)
        {
            model.UserId = user.Id;
            model.User = user;
            model.CreatedTime = DateTime.UtcNow;
            model.Published = false;
            _db.Vacancies.Add(model);
            user.Vacancies.Add(model);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Profile", "Employer");
        }
        ModelState.AddModelError("", "Ошибка при создании!");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        Vacancy? vacancy = await _db.Vacancies.FirstOrDefaultAsync(r => r.Id == id);
        vacancy.EditedTime = DateTime.UtcNow;
        _db.Vacancies.Update(vacancy);
        await _db.SaveChangesAsync();
        return Ok(vacancy.EditedTime.ToString());
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Public(int id)
    {
        Vacancy? vacancy = await _db.Vacancies.FirstOrDefaultAsync(r => r.Id == id);
        if (vacancy != null)
        {
            vacancy.Published = !vacancy.Published;
            _db.Vacancies.Update(vacancy);
            await _db.SaveChangesAsync();
            return Ok(vacancy.Published);
        }
        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        User? user = await _userManager.GetUserAsync(User);
        Vacancy? vacancy = await _db.Vacancies.FirstOrDefaultAsync(r => r.Id == id);
        if (vacancy != null)
        {
            user.Vacancies.Remove(vacancy);
            _db.Vacancies.Remove(vacancy);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return Ok(true);
        }
        return Ok(false);
    }
}