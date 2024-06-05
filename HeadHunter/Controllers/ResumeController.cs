using System.Text.Json.Serialization;
using HeadHunter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HeadHunter.Controllers;

public class ResumeController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public ResumeController(HeadHunterDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Resume model, string? modulesObjs )
    {
        User user = await _userManager.GetUserAsync(User);
        if (ModelState.IsValid)
        {
            var buffer = JsonConvert.DeserializeObject<List<ModuleViewModel>>(modulesObjs);
            var modules = new List<Module>();
            Resume resume = model;
            resume.UserId = user.Id;
            resume.User = user;
            resume.Published = false;
            resume.CreatedTime = DateTime.UtcNow;
            foreach (var buff in buffer)
            {
                Module module = new Module()
                {
                    StartedWorking = DateTime.Parse(buff.StartDate),
                    EndedWorking = DateTime.Parse(buff.EndDate),
                    OrganizationName = buff.Organization,
                    Role = buff.Role,
                    Responsibilities = buff.Response,
                    Identity = buff.Identity
                };
                modules.Add(module);
            }
            resume.Modules = modules;
            foreach (var module in modules)
                _db.Modules.Add(module);
            _db.Resumes.Add(resume);
            user.Resumes.Add(resume);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            ModelState.AddModelError("", "Резюме создан успешно!");
            return View();
        }
        ModelState.AddModelError("", "Не получилось создать резюме!");
        return View(model);
    }

}