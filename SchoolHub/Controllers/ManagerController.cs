using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto.School;
using SchoolHub.Interface;
using System.Security.Claims;

namespace SchoolHub.Controllers
{
    [Authorize]
    public class ManagerController(ISchoolService schoolService,IGeneralService generalService) : Controller
    {
        public async Task<IActionResult> SchoolInfo(int schoolId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isManager = schoolService.IsManagerOfSchool(managerUserId:userId , schoolId:schoolId);
            if (!isManager)
            return RedirectToAction("SchoolPage","School");
            else
            {
                var schoolData = new AllDatailAboutSchool();

                schoolData.School = await generalService.GetFullDataOfSchoolById(schoolId);
                schoolData.Classes = schoolService.GetClasses(schoolId);
                return View(schoolData);
            }
        }
    }
}
