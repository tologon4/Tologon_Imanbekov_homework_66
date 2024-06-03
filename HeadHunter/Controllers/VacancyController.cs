using Microsoft.AspNetCore.Mvc;

namespace HeadHunter.Controllers;

public class VacancyController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}