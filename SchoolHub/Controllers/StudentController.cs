using Microsoft.AspNetCore.Mvc;

namespace SchoolHub.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
