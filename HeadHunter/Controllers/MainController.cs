using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HeadHunter.Controllers;

public class MainController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public MainController(HeadHunterDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }
    
    
    public async Task<IActionResult> Index(string? filter)
    {
        ViewBag.Categories = _db.Categories.Select(c => c.Name).ToList();
        int? userId;
        try
        {
            userId = int.Parse(_userManager.GetUserId(User));
        }
        catch(Exception ex)
        {
            userId = 0;
        }
        List<Vacancy> vacancies = new List<Vacancy>();
        User currentUser = new User();
        if (userId > 0)
        {
            User? user = await _db.Users.Include(r => r.Resumes).FirstOrDefaultAsync(u => u.Id == userId);
            if (user.Role == "Соискатель" && user.Resumes.Where(r => r.Published == true).Count()>0)
                vacancies = _db.Vacancies.Include(u => u.User)
                    .Where(v => v.Published == true)
                    .OrderByDescending(v => v.EditedTime).ToList();;
            currentUser = await _db.Users.Include(r => r.Resumes)
                .FirstOrDefaultAsync(u => u.Role == "Соискатель" && u.Id == userId);
            if (currentUser != null)
            {
                var curUserResumes = _db.Resumes
                    .Where(r => r.Published == true && r.UserId == currentUser.Id)
                    .Select(r => new { r.Id, r.Title, r.UserId})
                    .ToList();

                ViewBag.CurrentUser = currentUser;
                ViewBag.CurrentUserId = currentUser.Id;
                ViewBag.CurrentUserResumes = JsonConvert.SerializeObject(curUserResumes);
            }
            
        }

        if (filter != null)
        {
            vacancies = vacancies.Where(v => v.Category == filter).ToList();
        }
        return View(vacancies);
    }
}