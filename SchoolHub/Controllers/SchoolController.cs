using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using SchoolHub.Models;
using System.Security.Claims;

namespace SchoolHub.Controllers
{
    [Authorize]
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

        [HttpPost]
        public IActionResult AddSchool(SchoolDto school)
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            school.ManagerUserId= userId;
             var validation = schoolService.AddSchool(school);
            if (validation == false)
            {
                ViewBag.Eror = "مدرسه تکراری هست";
                return RedirectToAction("SchoolPage");
            }
            else
            {
                return RedirectToAction("SchoolPage");

            }

        }

      
        [HttpPost]
        public IActionResult AddClass(ClassDto classDto)
        {
            
             var validation = schoolService.AddClass(classDto);
                if (validation == false)
                {
                    ViewBag.Eror = "کلاس تکراری هست";
                    return View();
                }
                else
                {
                    return RedirectToAction("SchoolInfo", new { id = classDto.SchoolId });

                }
        }

        [HttpPost]
        public IActionResult DeleteClass(int id)
        {
            var validation = schoolService.DeleteClass(id);
                return Json(validation);
            
        }

        public IActionResult GetGenerals()
        {
            var genrals = schoolService.GetGeneralItems(new string[]
            {
                "نوع","استان","جنسیت","دوره تحصیلی","مقطع تحصیلی","شیفت"
            });
            AddSchoolViewModel addSchoolViewModel = new AddSchoolViewModel
            {
                GeneralEducationLevels = genrals.FirstOrDefault(x=> x.Key == "مقطع تحصیلی").Value,
                GeneralEducationPeriods = genrals.FirstOrDefault(x=> x.Key == "دوره تحصیلی").Value,
                GeneralGender = genrals.FirstOrDefault(x=> x.Key == "جنسیت").Value,
                GeneralProrvince = genrals.FirstOrDefault(x=> x.Key == "استان").Value,
                GeneralShifts = genrals.FirstOrDefault(x=> x.Key == "شیفت").Value,
                GeneralTypes = genrals.FirstOrDefault(x=> x.Key == "نوع").Value
           
            };
            return Json(addSchoolViewModel);
        } 

        public IActionResult GetCities(int provinceId)
        {
            return Json(schoolService.GetCitiesByProvinceId(provinceId));
        } 

        public IActionResult GetDistricts(int CityId)
        {
             return Json(schoolService.GetDistrictByCityId(CityId));
        } 
    }
}