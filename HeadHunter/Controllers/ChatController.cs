using HeadHunter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeadHunter.Controllers;

public class ChatController : Controller
{
    private HeadHunterDb _db;
    private UserManager<User> _userManager;

    public ChatController(HeadHunterDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> ChatResults(int? vacancyId, int? applicantId)
    {
         User curUser = _db.Users.FirstOrDefault(u => u.Id == int.Parse(_userManager.GetUserId(User)));
         ViewBag.CurrentUser = curUser;
        if (vacancyId.HasValue)
        {
            Chat? chat = await _db.Chats.Include(u => u.Applicant)
                .Include(r => r.Messages)
                .Include(v => v.Vacancy)
                .FirstOrDefaultAsync(c => c.VacancyId == vacancyId && c.ApplicantId == applicantId);
            var messages = _db.Messages.Include(u => u.User)
                .Include(r => r.Resume)
                .Where(m => m.ChatId == chat.Id).ToList();
            return PartialView("_ChatPartialView", messages);
        }
        return NotFound();
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(int? vacancyId, int? resumeId, string? messageContent, int? applicantId)
    {
        User? currentUser = await _userManager.GetUserAsync(User);
        Vacancy? vacancy = await _db.Vacancies
            .Include(v => v.Chats)
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
        Resume? resume = await _db.Resumes
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == resumeId);
        User? applicant = await _db.Users
            .Include(u => u.Resumes)
            .FirstOrDefaultAsync(u => u.Id == applicantId);
        Chat? chat = await _db.Chats
            .Include(c => c.Vacancy)
            .Include(c => c.Applicant)
            .FirstOrDefaultAsync(c => c.VacancyId == vacancyId);

        if (vacancy != null && applicant != null)
        {
            if (chat == null)
            {
                chat = new Chat()
                {
                    VacancyId = vacancy.Id,
                    Vacancy = vacancy,
                    ApplicantId = applicant.Id,
                    Applicant = applicant,
                };
                _db.Entry(chat).State = EntityState.Added; 
            }

            Message message = new Message()
            {
                UserId = currentUser?.Id == vacancy.UserId ? vacancy.UserId : resume?.UserId,
                MessageContent = messageContent,
                ResumeId = resume?.Id,
                ChatId = chat.Id,
                Chat = chat,
                SentTime = DateTime.UtcNow
            };

            if (applicant.Chats == null || !applicant.Chats.Contains(chat))
            {
                applicant.Chats ??= new List<Chat>();
                applicant.Chats.Add(chat);
            }

            _db.Messages.Add(message);
            chat.Messages.Add(message);
            vacancy.Chats.Add(chat);
            _db.Vacancies.Update(vacancy);
            _db.Users.Update(applicant);

            await _db.SaveChangesAsync();
            return Ok(message);
        }
        return NotFound();
    }
}