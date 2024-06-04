using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        User user = await _userManager.GetUserAsync(User);
        return View(user);
    }
}