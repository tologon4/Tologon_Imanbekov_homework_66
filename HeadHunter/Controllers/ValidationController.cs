using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeadHunter.Controllers;

public class ValidationController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public ValidationController(HeadHunterDb context, UserManager<User> userManager)
    {
        _db = context;
        _userManager = userManager;
    }
    
    [AcceptVerbs("GET", "POST")]
    public bool CheckEmail(string Email)
    {
        return !_db.Users.Any(u => u.Email.ToLower().Trim() == Email.ToLower().Trim());
    }
    [AcceptVerbs("GET", "POST")]
    public bool CheckUsername(string UserName)
    {
        return !_db.Users.Any(u => u.UserName.ToLower().Trim() == UserName.ToLower().Trim());

    }
    
    [HttpGet]
    public async Task<IActionResult> EditCheckUsernameEmail(string value)
    {
        User usr = await _userManager.GetUserAsync(User);
        bool result = true;
        if (_db.Users.Any(u => u.UserName.ToLower().Trim() == value.ToLower().Trim()) || _db.Users.Any(u => u.Email.ToLower().Trim() == value.ToLower().Trim()))
        {
            if (value.ToLower().Trim() == usr.UserName.ToLower().Trim() || value.ToLower().Trim() == usr.Email.ToLower().Trim())
                result = true;
            else
                result = false;
        }
        return Ok(result);
    }
}