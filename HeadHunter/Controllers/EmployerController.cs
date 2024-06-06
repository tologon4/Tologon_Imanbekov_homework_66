using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeadHunter.Controllers;

public class EmployerController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public EmployerController(HeadHunterDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Profile()
    {
        User user = await _db.Users.Include(v => v.Vacancies)
            .FirstOrDefaultAsync(u => u.Id == int.Parse(_userManager.GetUserId(User)));
        return View(user);
    }
}