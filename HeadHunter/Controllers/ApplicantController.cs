using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeadHunter.Controllers;

public class ApplicantController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public ApplicantController(HeadHunterDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Profile()
    {
        User user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string key, string value)
    {
        User? user = await _userManager.GetUserAsync(User);
        bool result;
        switch (key)
        {
            case "username":
                if (_db.Users.Any(u => u.UserName == value))
                {
                    if (value.ToLower().Trim() == user.UserName.ToLower().Trim())
                    {
                        user.UserName = value;
                        result = true;
                    }
                    else
                        result = false;
                }
                else
                {
                    user.UserName = value;
                    result = true;
                }
                break;
            case "email":
                if (_db.Users.Any(u => u.Email == value))
                {
                    if (value.ToLower().Trim() == user.Email.ToLower().Trim())
                    {
                        user.Email = value;
                        result = true;
                    }
                    else
                        result = false;
                }
                else
                {
                    user.Email = value;
                    result = true;
                }
                break;
            case "phone":
                user.PhoneNumber = value;
                result = true;
                break;
            case "password":
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, value);
                result = true;
                break;
            default:
                result = false;
                break;
        }
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(result);
    }
}