using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SchoolHub.ViewComponents;
using SchoolHub.Interface;
namespace SchoolHub.ViewComponents
{
    public class SidebarViewComponent(IMenuService menuService) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = User as ClaimsPrincipal;

            var userId = 0;

            int.TryParse(
             claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier),
             out userId
            );

            var schoolBar = menuService.GetSchoolBar(userId);

            return View(schoolBar);
        }
    }
}
