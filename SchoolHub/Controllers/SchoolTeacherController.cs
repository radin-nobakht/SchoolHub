using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Dto.Teacher;
using SchoolHub.Interface;

namespace SchoolHub.Controllers
{
    public class SchoolTeacherController(
        ISchoolTeacherService teacherService,
        IGeneralService generalService
    ) : Controller
    {
        public async Task<IActionResult> SchoolInfo(int schoolId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isTeacher = teacherService.IsTeacherOfschool(
                teacherUserId: userId,
                schoolId: schoolId
            );

            if (!isTeacher)
            {
                TempData["Message"] = "شما اجازه دسترسی به این صفحه را ندارید.";
                return RedirectToAction("SchoolPage", "School");
            }
            else
            {
                var schoolDetail = new AllDatailAboutSchool();
                schoolDetail.School = await generalService.GetFullDataOfSchoolById(schoolId);
                schoolDetail.Classes = teacherService.GetTeacherClasses(
                    teacherUserId: userId,
                    schoolId: schoolId
                );
                return View(schoolDetail);
            }
        }

        public IActionResult ClassInfo(int classId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var schoolId = teacherService.GetSchoolIdByClassId(classId);
            var isTeacher = teacherService.IsTeacherOfClass(
                teacherUserId: userId,
                classId: classId
            );
            if (!isTeacher)
            {
                TempData["Message"] = "شما اجازه دسترسی به این کلاس را ندارید.";
                return RedirectToAction("SchoolInfo", new { schoolId = schoolId, userId = userId });
            }
            else
            {
                var teacherClassInfo = new TeacherClassInfoDto { ClassId = classId };
                teacherClassInfo = teacherService.GetClassDetail(classId);
                teacherClassInfo.Students = teacherService.GetStudentsDetail(classId);
                teacherClassInfo.SchoolName = teacherService.GetSchoolName(schoolId);
                teacherClassInfo.Subjects = teacherService.GetSubjectDetail(userId, classId);
                return View(teacherClassInfo);
            }
        }
    }
}
