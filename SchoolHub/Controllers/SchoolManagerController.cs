using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using SchoolHub.Dto.Manager;

namespace SchoolHub.Controllers
{
    [Authorize]
    public class SchoolManagerController(
        IGeneralService generalService,
        ISchoolManagerService schoolManagerService
    ) : Controller
    {
        public async Task<IActionResult> SchoolInfo(int schoolId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isManager = schoolManagerService.IsManagerOfSchool(
                managerUserId: userId,
                schoolId: schoolId
            );
            if (!isManager)
            {
                TempData["Eror"] = "شما اجازه دسترسی به این صفحه را ندارید.";
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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
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

        public IActionResult GetMajorForAddClass(int generalGradeId)
        {
            var majors = generalService.GetGenerals("Major", generalGradeId);

            return Json(majors);
        }

        public IActionResult GetGradesForAddClass(int schoolId)
        {
            var grades = generalService.GetGeneralGrades(schoolId);

            return Json(grades);
        }

        [HttpPost]
        public IActionResult DeleteClass(int classId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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

        public IActionResult ClassInfo(int classId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var schoolId = schoolManagerService.GetSchoolIdByClassId(classId) ?? 0;
            var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId, userId);
            if (!isManager)
            {
                TempData["Eror"] = "شما اجازه دسترسی به این کلاس را ندارید.";
                return RedirectToAction("SchoolInfo", "Manager", new { schoolId = schoolId });
            }
            else
            {
                var managerClassInfo = new ManagerClassInfoDto { ClassId = classId, SchoolId = schoolId };
                managerClassInfo.Average = schoolManagerService.GetClassAverage(classId);
                managerClassInfo.Students = schoolManagerService.GetStudentsDetail(classId);
                managerClassInfo.Teachers = schoolManagerService.GetTeachersDetail(classId);
                managerClassInfo.Class = schoolManagerService.GetClassDetail(classId);
                managerClassInfo = schoolManagerService.GetManagerClassDetail(managerClassInfo);
                return View(managerClassInfo);
            }
        }


        public IActionResult UpdateClass(ClassDto clas)
        {
            var msg = schoolManagerService.UpdateClass(clas);
            if (msg.success)
                TempData["Success"] = msg.message;
            else
                TempData["Eror"] = msg.message;



            return RedirectToAction("ClassInfo", "SchoolManager", new { classId = clas.Id });
        }


        public IActionResult GetMajorForUpdateClass(int classId, int generalGradeId)
        {

            var currentIem = generalService.GetCurrentItem(classId, "Major");
            var majors = generalService.GetGenerals("Major", generalGradeId);

            return Json(new
            {
                Majors = majors,
                CurrentIem = currentIem
            });
        }
        public IActionResult GetGradeForUpdateClass(int classId, int schoolId)
        {
            var currentIem = generalService.GetCurrentItem(classId, "Grade");
            var grades = generalService.GetGeneralGrades(schoolId);

            return Json(new
            {
                Grades = grades,
                CurrentIem = currentIem
            });
        }



        public IActionResult AddStudent(int classId,string nationalId)
        {
            var isAdd= schoolManagerService.AddStudent(new StudentDto { ClassId = classId} , nationalId);
            if (isAdd.IsCorrect)
            {
                TempData["Success"] = isAdd.message;
            }
            else
            {
                TempData["Eror"] = isAdd.message;
            }
            return RedirectToAction("ClassInfo", "SchoolManager", new { classId = classId });
        }





    }
}
