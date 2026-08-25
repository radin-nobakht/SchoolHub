using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SchoolHub.Controllers
{
    [Authorize]
    public class SchoolManagerController(
        ISchoolService schoolService,
        IGeneralService generalService,
        ISchoolManagerService schoolManagerService
    ) : Controller
    {
        public async Task<IActionResult> SchoolInfo(int schoolId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isManager = schoolManagerService.IsManagerOfSchool(
                managerUserId: userId,
                schoolId: schoolId
            );
            if (!isManager)
            {
                TempData["Message"] = "شما اجازه دسترسی به این صفحه را ندارید.";
                return RedirectToAction("SchoolPage", "School");
            }
            else
            {
                var schoolData = new AllDatailAboutSchool();

                schoolData.School = await generalService.GetFullDataOfSchoolById(schoolId);
                schoolData.Classes = schoolManagerService.GetAllClasses(schoolId);
                return View(schoolData);
            }
        }

        [HttpPost]
        public IActionResult AddClass(ClassDto classDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var schoolId = schoolManagerService.GetSchoolIdByClassId(classDto.Id);

            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId ?? 0, userId);

            if (!isManager)
            {
                TempData["Error"] = "شما اجازه اضافه کردن کلاس را ندارید";
            }
            else
            {
                var validation = schoolManagerService.AddClass(classDto);

                if (!validation)
                    TempData["Error"] = "کلاس تکراری است.";
            }

            return RedirectToAction(
                "SchoolInfo",
                "SchoolManager",
                new { schoolId = classDto.SchoolId }
            );
        }

        public IActionResult GetMajor(int generalGradeId)
        {
            var majors = generalService.GetGeneralMajor(generalGradeId);

            return Json(majors);
        }

        public IActionResult GetGrades(int schoolId)
        {
            var grades = generalService.GetGeneralGrades(schoolId);

            return Json(grades);
        }

        [HttpPost]
        public IActionResult DeleteClass(int classId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var schoolId = schoolManagerService.GetSchoolIdByClassId(classId);
            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId ?? 0, userId);
            if (!isManager)
            {
                return Json(false);
            }
            else
            {
                var validation = schoolManagerService.DeleteClass(classId);
                return Json(validation);
            }
        }
    }
}
