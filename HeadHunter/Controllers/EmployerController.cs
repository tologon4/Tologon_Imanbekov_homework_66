using System.Formats.Asn1;
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

    public async Task<IActionResult> Profile(int? id)
    {
        if (id.HasValue)
        {
            User user = await _db.Users.Include(v => v.Vacancies)
                .FirstOrDefaultAsync(u => u.Id == id);
            ViewBag.CurrentUser = await _userManager.GetUserAsync(User);
            return View(user);
        }
        return NotFound();
    }

    public async Task<IActionResult> Applicants()
    {
        var users = _db.Users.Where(u => u.Role == "Соискатель").ToList();
        return View(users);
    }

}