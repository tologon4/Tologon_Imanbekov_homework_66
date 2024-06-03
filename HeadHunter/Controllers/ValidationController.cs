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
    
    [AcceptVerbs("GET", "POST")]
    public bool EditCheckEmail(string Email)
    {
        User usr = _db.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User)));
        bool result = true;
        if (_db.Users.Any(u => u.Email.ToLower().Trim() == Email.ToLower().Trim()))
        {
            if (Email.ToLower().Trim() == usr.Email.ToLower().Trim())
                result = true;
            else
                result = false;
        }
        return result;
    }
    [AcceptVerbs("GET", "POST")]
    public bool EditCheckUsername(string UserName)
    {
        User usr = _db.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User)));
        bool result = true;
        if (_db.Users.Any(u => u.UserName.ToLower().Trim() == UserName.ToLower().Trim()))
        {
            if (UserName.ToLower().Trim() == usr.UserName.ToLower().Trim())
                result = true;
            else
                result = false;
        }
        return result;
    }
}