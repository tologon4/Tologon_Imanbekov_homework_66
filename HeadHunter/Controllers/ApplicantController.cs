using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeadHunter.Controllers;

public class ApplicantController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;
    private IWebHostEnvironment _environment;

    public ApplicantController(HeadHunterDb db, UserManager<User> userManager, IWebHostEnvironment environment)
    {
        _db = db;
        _userManager = userManager;
        _environment = environment;
    }

    public async Task<IActionResult> Profile()
    {
        User user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string key, string? value, IFormFile? uploadedFile)
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
            case "avatar":
                var buffer = user.Avatar.Split('=');
                var buffer2 = buffer[buffer.Length - 1].Split('.');
                string newFileName = Path.ChangeExtension($"{user.UserName.Trim()}-ProfileN={int.Parse(buffer2[0])+1}", Path.GetExtension(uploadedFile.FileName));
                string path= $"/userImages/" + newFileName.Trim();
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                user.Avatar = path;
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
        return Ok(new {result, avatar = user.Avatar});
    }
}