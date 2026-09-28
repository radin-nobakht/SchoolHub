using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.Manager;
using SchoolHub.Dto.School;
using SchoolHub.Dto.Student;
using SchoolHub.Entity;
using SchoolHub.Interface;
using SchoolHub.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchoolHub.Controllers;

[Authorize]
public class SchoolManagerController(IGeneralService generalService, ISchoolManagerService schoolManagerService) : Controller
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
        var isSchoolExist = schoolManagerService.IsSchoolExist(classDto.SchoolId);

        var isManager = schoolManagerService.IsManagerOfSchoolClass(classDto.SchoolId, userId);

        if (!isManager || !isSchoolExist)
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
            managerClassInfo = schoolManagerService.GetManagerClassDetail(managerClassInfo);
            managerClassInfo.Average = schoolManagerService.GetClassAverage(classId);
            managerClassInfo.Students = schoolManagerService.GetStudentsDetail(classId);
            managerClassInfo.Teachers = schoolManagerService.GetTeachersDetail(classId);
            managerClassInfo.Class = schoolManagerService.GetClassDetail(classId);

            return View(managerClassInfo);
        }
    }

    public IActionResult UpdateClass(ClassDto clas)
    {
        var msg = schoolManagerService.UpdateClass(clas);
        if (msg.Success)
            TempData["Success"] = msg.Message;
        else
            TempData["Eror"] = msg.Message;



        return RedirectToAction("ClassInfo", "SchoolManager", new { classId = clas.Id });
    }

    public IActionResult GetMajorForUpdateClass(int classId, int generalGradeId)
    {

        var currentIem = generalService.GetCurrentItem(classId, "Major")?.Id ?? 0;
        var majors = generalService.GetGenerals("Major", generalGradeId);

        return Json(new
        {
            Majors = majors,
            CurrentIem = currentIem
        });
    }

    public IActionResult GetGradeForUpdateClass(int classId, int schoolId)
    {
        var currentIem = generalService.GetCurrentItem(classId, "Grade")?.Id ?? 0;
        var grades = generalService.GetGeneralGrades(schoolId);

        return Json(new
        {
            Grades = grades,
            CurrentIem = currentIem
        });
    }

    public IActionResult AddStudent(int classId, string nationalId)
    {
        var isAdd = schoolManagerService.AddStudent(new StudentDto { ClassId = classId }, nationalId);
        if (isAdd.Success)
        {
            TempData["Success"] = isAdd.Message;
        }
        else
        {
            TempData["Eror"] = isAdd.Message;
        }
        return RedirectToAction("ClassInfo", "SchoolManager", new { classId = classId });
    }

    public IActionResult GetAvailableSubjects(int classId)
    {
        var gradeId = schoolManagerService.GetGradeIdByClassId(classId);
        var scores = generalService.GetAvailableSubjectsByGradeId(gradeId: gradeId ?? 0, classId: classId);
        return Json(scores);
    }

    public IActionResult AddTeacherToClass(Dto.Manager.TeacherDto addTeacher)
    {
        var isAdd = schoolManagerService.AddTeacher(addTeacher);
        if (isAdd.Success)
            TempData["Success"] = isAdd.Message;
        else
            TempData["Eror"] = isAdd.Message;

        return RedirectToAction("ClassInfo", "SchoolManager", new { classId = addTeacher.ClassId });
    }

    public IActionResult DeleteStudent(int studentId)
    {
        var isDeleted = schoolManagerService.DeleteStudent(studentId);
        return Json(isDeleted);
    }

    public IActionResult DeleteTeacher(int classId, int teacherUserId)
    {
        var isDeleted = schoolManagerService.DeleteTeacher(classId: classId, teacherUserId: teacherUserId);
        return Json(isDeleted);
    }

    public IActionResult UpdateTeacher(Dto.Manager.TeacherDto teacher)
    {
        var isAdd = schoolManagerService.UpdateTeacher(teacher);
        if (isAdd.Success)
            TempData["Success"] = isAdd.Message;
        else
            TempData["Eror"] = isAdd.Message;

        return RedirectToAction("ClassInfo", "SchoolManager", new { classId = teacher.ClassId });
    }

    public IActionResult GetSubjectsForUpdateTeacher(int classId, int teacherUserId)
    {
        var gradeId = schoolManagerService.GetGradeIdByClassId(classId);
        var subjects = generalService.GetAvailableSubjectsByGradeIdForUpdateTeacher(gradeId ?? 0, classId, teacherUserId);
        var teacherSubjectIds = generalService.GetTeacherSubjects(classId, teacherUserId).Select(x => x.Id);
        return Json(new { AvailableSubjects = subjects, TeacherSubjectIds = teacherSubjectIds });
    }

    public IActionResult StudentInfo(int schoolId, int studentUserId)
    {

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isManager = schoolManagerService.IsManagerOfSchoolClass(schoolId, userId);

        var classId = schoolManagerService.GetClassIdByStudentAndSchoolId(studentUserId, schoolId);
        var studentId = schoolManagerService.GetStudentIdByStudentAndClassId(studentUserId, classId);
        if (classId == 0 && studentId == 0 && !isManager)
        {
            TempData["Eror"] = "شما اجازه دسترسی به این صفحه را ندارید.";
            return RedirectToAction("SchoolPage", "School");
        }
        else
        {
            var studentCard = new StudentReportCardDto();

            studentCard = schoolManagerService.GetClassDetailForManager(studentId);

            studentCard.OverallAverage = schoolManagerService.GetOverallAverage(studentId);

            studentCard.Subjects = schoolManagerService.GetSubjectsDetailForManager(studentId, classId);

            return View(studentCard);
        }


    }

    public IActionResult GetGeneralsForUpdateSchool(int schoolId)
    {

        var generals = generalService.GetGeneralItems(
                    new[] { "نوع", "استان", "جنسیت پذیرش", "دوره تحصیلی", "مقطع تحصیلی", "شیفت" }
                );

        var model = new AddSchoolViewModel
        {
            GeneralEducationLevels = generals.FirstOrDefault(x => x.Key == "EducationLevel").Value,

            GeneralEducationPeriods = generals.FirstOrDefault(x => x.Key == "EducationPeriod").Value,

            GeneralGender = generals.FirstOrDefault(x => x.Key == "AdmissionGender").Value,

            GeneralProrvince = generals.FirstOrDefault(x => x.Key == "Province").Value,

            GeneralShifts = generals.FirstOrDefault(x => x.Key == "Shift").Value,

            GeneralTypes = generals.FirstOrDefault(x => x.Key == "Type").Value,
        };

        var schoolProperty = generalService.GetGeneralIdsForSchool(schoolId);

        return Json(new { generals = model, schoolProperty = schoolProperty });
    }

    public IActionResult GetNationalId()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var nationalId = schoolManagerService.GetNationalId(userId);
        return Json(nationalId);
    }

    public IActionResult GetCities(int provinceId)
    {
        var cities = generalService.GetCitiesByProvinceId(provinceId);

        return Json(cities);
    }

    public IActionResult GetDistricts(int cityId)
    {
        var districts = generalService.GetDistrictByCityId(cityId);

        return Json(districts);
    }


    public IActionResult GetUserByNationalId(string nationalId)
    {
        var user = schoolManagerService.GetUserByNationalId(nationalId);
        return Json(user.Name + " " + user.LastName);
    }


    public IActionResult UpdateSchool(SchoolDto school, string managerNationalId, int schoolId)
    {
        school.Id = schoolId;
        school.ManagerUserId = schoolManagerService.GetUserByNationalId(managerNationalId)?.Id ?? 0;
        var isUpdate = schoolManagerService.UpdateSchool(school);
        if (isUpdate.IsCorrect)
        {
            TempData["Success"] = isUpdate.Message;
        }
        else
        {
            TempData["Eror"] = isUpdate.Message;
        }
        return RedirectToAction("SchoolInfo", new { schoolId = school.Id });
    }


    public IActionResult Teachers(int schoolId)
    {
        var teachersView = new TeachersViewModel { SchoolId = schoolId };
        teachersView.Teachers = schoolManagerService.GetTeachersBySchoolId(schoolId);
        teachersView.SchoolName = schoolManagerService.GetSchoolBySchoolId(schoolId)?.Name ?? "";
        return View(teachersView);
    }

    public IActionResult AddTeacherToSchool(string teacherNationalId,int schoolId)
    {
        var isAdd = schoolManagerService.AddTeacherToSchool(schoolId: schoolId, teacherNationalId: teacherNationalId);
        if (isAdd.IsCorrect)
        {
            TempData["Success"] = isAdd.Message;
        }
        else
        {
            TempData["Eror"] = isAdd.Message;
        }
        return RedirectToAction("Teachers",new {schoolId});
    }

    [HttpGet]
    public IActionResult GetTeachers(int classId)
    {
        var schoolId=schoolManagerService.GetSchoolIdByClassId(classId);
        var teacherIds = schoolManagerService.GetTeacherUserIds(schoolId ??0);
        var schoolTeachers = schoolManagerService.GetAvailableTeachersById(teacherIds,classId);
        return Json(schoolTeachers);
    }

    public IActionResult DaleteTeacherOfSchool(int teacherId)
    {
        return Json(schoolManagerService.DeleteTeacherOfSchool(teacherId));
    }
}
