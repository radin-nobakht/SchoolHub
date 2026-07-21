using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using System.Security.Claims;

namespace SchoolHub.Controllers
{
    public class SchoolController(ISchoolService schoolService) : Controller
    {
        public IActionResult SchoolPage()
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            var schools = schoolService.GetSchools(userId);
            return View(schools);
        }
        public IActionResult SchoolInfo(int id)
        {
            var school = new AllDatailAboutSchool();
            school.School = schoolService.GetSchoolById(id);
            school.Classes = schoolService.GetClasses(id);
            school.Manager = schoolService.GetManagerById(school.School.ManagerUserId);
            return View(school);
        }


    }
}
