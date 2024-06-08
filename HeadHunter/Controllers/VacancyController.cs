using HeadHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
    
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = _db.Categories.Select(c => c.Name).ToList();
        return View();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id.HasValue)
        {
            ViewBag.Categories = _db.Categories.Select(c => c.Name).ToList();
            User currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUser = currentUser;
            ViewBag.CurrentUserId = currentUser.Id;
            var curUserresumes = _db.Resumes
                .Where(r => r.Published == true)
                .Where(r => r.UserId == currentUser.Id)
                .Select(r => new { r.Id, r.Title, r.UserId }).ToList();
            ViewBag.CurrentUserResumes = JsonConvert.SerializeObject(curUserresumes);
            Vacancy vacancy = await _db.Vacancies.FirstOrDefaultAsync(v => v.Id == id);
            return View(vacancy);
        }
        return NotFound();
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
        ViewBag.Categories = _db.Categories.Select(c => c.Name).ToList();
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
            return RedirectToAction("Profile", "Employer", new {id = user.Id});
        }
        ModelState.AddModelError("", "Ошибка при создании!");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(int? id)
    {
        if (id.HasValue)
        {
            Vacancy? vacancy = await _db.Vacancies.FirstOrDefaultAsync(r => r.Id == id);
            vacancy.EditedTime = DateTime.UtcNow;
            _db.Vacancies.Update(vacancy);
            await _db.SaveChangesAsync();
            return Ok(vacancy.EditedTime.ToString());
        }
        return NotFound();
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