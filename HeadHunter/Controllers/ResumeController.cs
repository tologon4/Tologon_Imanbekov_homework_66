using System.Text.Json.Serialization;
using HeadHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HeadHunter.Controllers;

public class ResumeController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;
    private IWebHostEnvironment _environment;

    public ResumeController(HeadHunterDb db, UserManager<User> userManager, IWebHostEnvironment environment)
    {
        _db = db;
        _userManager = userManager;
        _environment = environment;
    }


    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        Resume resume = await _db.Resumes.Include(m => m.Modules).FirstOrDefaultAsync(r => r.Id == id);
        return View(resume);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = _db.Categories.Select(c => c.Name).ToList();
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(int resumeId, int? moduleId, string key, string? value, IFormFile? uploadedFile)
    {
        User? user = await _userManager.GetUserAsync(User);
        Resume? resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId);
        Module? module = await _db.Modules.FirstOrDefaultAsync(m => m.Id == moduleId);
        bool result;
        switch (key)
        {
            case "role":
                module.Role = value;
                result = true;
                break;
            case "organ":
                module.OrganizationName = value;
                result = true;
                break;
            case "facebook":
                resume.Facebook = value;
                result = true;
                break;
            case "linkedIn":
                resume.LinkedIn = value;
                result = true;
                break;
            case "response":
                module.Responsibilities = value;
                result = true;
                break;
            case "startedDate":
                module.StartedWorking = DateTime.Parse(value);
                result = true;
                break;
            case "endedDate":
                module.EndedWorking = DateTime.Parse(value);
                result = true;
                break;
            case "email":
                resume.Email = value;
                result = true;
                break;
            case "firstName":
                resume.UserFirstName = value;
                result = true;
                break;
            case "lastName":
                resume.UserLastName = value;
                result = true;
                break;
            case "phone":
                resume.PhoneNumber = value;
                result = true;
                break;
            case "avatar":
                var buffer = resume.UserAvatar.Split('=');
                var buffer2 = buffer[buffer.Length - 1].Split('.');
                string newFileName = Path.ChangeExtension($"{resume.UserLastName.Trim()}-ResumeN={int.Parse(buffer2[0])+1}", Path.GetExtension(uploadedFile.FileName));
                string path= $"/userImages/" + newFileName.Trim();
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                resume.UserAvatar = path;
                result = true;
                break;
            case "title":
                resume.Title = value;
                result = true;
                break;
            case "category":
                resume.Category = value;
                result = true;
                break;
            case "salary":
                resume.ExpectedSalary = int.Parse(value);
                result = true;
                break;
            default:
                result = false;
                break;
        }
        resume.EditedTime = DateTime.UtcNow;
        if (moduleId != null)
            _db.Modules.Update(module);
        _db.Resumes.Update(resume);
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return Ok(new {result, avatar = user.Avatar});
    }

    [HttpPost]
    public async Task<IActionResult> Create(Resume model, string? modulesObjs )
    {
        ViewBag.Categories = _db.Categories.Select(c => c.Name).ToList();
        User user = await _userManager.GetUserAsync(User);
        if (ModelState.IsValid)
        {
            List<ModuleViewModel> buffer = new List<ModuleViewModel>();
            if (modulesObjs != null)
            {
                buffer = JsonConvert.DeserializeObject<List<ModuleViewModel>>(modulesObjs);
            }
            var modules = new List<Module>();
            Resume resume = model;
            resume.UserId = user.Id;
            resume.User = user;
            resume.UserAvatar = user.Avatar;
            resume.Published = false;
            resume.CreatedTime = DateTime.UtcNow;
            if (buffer.Count > 0 )
            {
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
            }
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        Resume resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == id);
        resume.EditedTime = DateTime.UtcNow;
        _db.Resumes.Update(resume);
        await _db.SaveChangesAsync();
        return Ok(resume.EditedTime.ToString());
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Public(int id)
    {
        Resume? resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == id);
        if (resume != null)
        {
            resume.Published = !resume.Published;
            _db.Resumes.Update(resume);
            await _db.SaveChangesAsync();
            return Ok(resume.Published);
        }
        return NotFound();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int? id)
    {
        User? user = await _userManager.GetUserAsync(User);
        Resume? resume = await _db.Resumes.Include(m => m.Modules).FirstOrDefaultAsync(r => r.Id == id);
        if (resume != null)
        {
            _db.Modules.RemoveRange(resume.Modules);
            user.Resumes.Remove(resume);
            _db.Resumes.Remove(resume);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return Ok(true);
        }
        return Ok(false);
    }

}