using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using SchoolHub.Models;
using System.Security.Claims;

namespace SchoolHub.Controllers;

[Authorize]
public class SchoolController(ISchoolService schoolService, IGeneralService generalService) : Controller
{
    // =========================================================
    // Teacher - اطلاعات یک کلاس
    // =========================================================

    public IActionResult TeacherClassInfo(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        // بررسی اینکه این Teacher واقعاً این کلاس را تدریس می‌کند
        if (!schoolService.IsTeacherAssignedToClass(userId, id))
            return Forbid();

        var classInfo = schoolService.GetClassById(id);

        if (classInfo == null)
            return NotFound();

        var students = schoolService.GetStudentByClassId(id);

        // فقط Assignmentهای Teacher فعلی
        var teachers = schoolService.GetMySubjectsByClassId(
            id,
            userId);

        var model = new ClassInfoViewModel
        {
            Class = classInfo,
            Students = students,
            Teachers = teachers
        };

        return View(model);
    }


    // =========================================================
    // لیست مدارس کاربر
    // =========================================================

    public IActionResult SchoolPage()
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userId))
        {
            return Unauthorized();
        }

        var schools = schoolService.GetSchools(userId);

        return View(schools);
    }



    // =========================================================
    // اطلاعات کلاس - Manager
    // =========================================================

    public IActionResult ClassInfo(int id)
    {
        var classInfo = schoolService.GetClassById(id);

        if (classInfo == null)
            return NotFound();

        var model = new ClassInfoViewModel
        {
            Class = classInfo,
            Students = schoolService.GetStudentByClassId(id),
            Teachers = schoolService.GetTeacherByClassId(id)
        };

        return View(model);
    }


    // =========================================================
    // افزودن مدرسه
    // =========================================================

    [HttpPost]
    public IActionResult AddSchool(SchoolDto school)
    {
        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out int userId))
        {
            return Unauthorized();
        }

        school.ManagerUserId = userId;

        var validation = schoolService.AddSchool(school);

        if (!validation)
        {
            TempData["Error"] = "مدرسه تکراری است.";
        }

        return RedirectToAction(nameof(SchoolPage));
    }


    // =========================================================
    // افزودن کلاس
    // =========================================================

    //[HttpPost]
    //public IActionResult AddClass(ClassDto classDto)
    //{
    //    var validation = schoolService.AddClass(classDto);

    //    if (!validation)
    //        TempData["Error"] = "کلاس تکراری است.";

    //        return RedirectToAction(
    //            "SchoolInfo","",
    //            new
    //            {
    //                id = classDto.SchoolId,
    //                mode = "manager"
    //            });
        

       
    //}


    // =========================================================
    // حذف کلاس
    // =========================================================

    [HttpPost]
    public IActionResult DeleteClass(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var validation = schoolService.DeleteClass(id, userId);

        return Json(validation);
    }


    // =========================================================
    // دریافت GeneralItems مربوط به مدرسه
    // =========================================================

    public IActionResult GetGeneralsSchool()
    {
        var generals = generalService.GetGeneralItems(
            new[]
            {
                "نوع",
                "استان",
                "جنسیت پذیرش",
                "دوره تحصیلی",
                "مقطع تحصیلی",
                "شیفت"
            });

        var model = new AddSchoolViewModel
        {
            GeneralEducationLevels =
                generals.FirstOrDefault(
                    x => x.Key == "مقطع تحصیلی").Value,

            GeneralEducationPeriods =
                generals.FirstOrDefault(
                    x => x.Key == "دوره تحصیلی").Value,

            GeneralGender =
                generals.FirstOrDefault(
                    x => x.Key == "جنسیت").Value,

            GeneralProrvince =
                generals.FirstOrDefault(
                    x => x.Key == "استان").Value,

            GeneralShifts =
                generals.FirstOrDefault(
                    x => x.Key == "شیفت").Value,

            GeneralTypes =
                generals.FirstOrDefault(
                    x => x.Key == "نوع").Value
        };

        return Json(model);
    }


    // =========================================================
    // دریافت شهرها
    // =========================================================

    public IActionResult GetCities(int provinceId)
    {
        var cities = generalService.GetCitiesByProvinceId(provinceId);

        return Json(cities);
    }


    // =========================================================
    // دریافت مناطق
    // =========================================================

    public IActionResult GetDistricts(int cityId)
    {
        var districts = generalService.GetDistrictByCityId(cityId);

        return Json(districts);
    }


    // =========================================================
    // دریافت پایه‌ها
    // =========================================================

    public IActionResult GetGrades(int schoolId)
    {
        var grades = generalService.GetGeneralGrades(schoolId);

        return Json(grades);
    }



    public IActionResult GetMajor(int generalGradeId)
    {
        var majors = generalService.GetGeneralMajor(generalGradeId);

        return Json(majors);
    }
}

