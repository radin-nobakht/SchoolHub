using Microsoft.AspNetCore.Mvc;

namespace SchoolHub.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
