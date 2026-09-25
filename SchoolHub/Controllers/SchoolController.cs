using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using SchoolHub.Models;

namespace SchoolHub.Controllers;

[Authorize]
public class SchoolController(ISchoolService schoolService, IGeneralService generalService)
    : Controller
{
    // =========================================================
    // لیست مدارس کاربر 
    // =========================================================

    public IActionResult SchoolPage()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
            Teachers = schoolService.GetTeacherByClassId(id),
        };

        return View(model);
    }

    // =========================================================
    // افزودن مدرسه
    // =========================================================

    [HttpPost]
    public IActionResult AddSchool(SchoolDto school)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
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
    // حذف کلاس
    // =========================================================

    // =========================================================
    // دریافت GeneralItems مربوط به مدرسه
    // =========================================================

    public IActionResult GetGeneralsSchool()
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
}
