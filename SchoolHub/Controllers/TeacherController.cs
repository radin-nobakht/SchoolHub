using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using System.Security.Claims;


namespace SchoolHub.Controllers
{
    public class TeacherController(ISchoolService schoolService,IGeneralService generalService) : Controller
    {
        public async Task<IActionResult> SchoolInfo(int schoolId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isTeacher = schoolService.IsTeacherOfschool(teacherUserId: userId, schoolId: schoolId);

            if (!isTeacher)
            {
                TempData["Message"] = "شما اجازه دسترسی به این صفحه را ندارید.";
                return RedirectToAction("SchoolPage", "School");
            }
            else
            {
                var schoolDetail = new AllDatailAboutSchool();
                schoolDetail.School =await generalService.GetFullDataOfSchoolById(schoolId);
                schoolDetail.Classes = schoolService.GetTeacherClasses(teacherUserId: userId, schoolId: schoolId);
                return View(schoolDetail);
            }
        }
    }
}
