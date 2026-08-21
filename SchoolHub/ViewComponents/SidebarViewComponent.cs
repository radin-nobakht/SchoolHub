using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Interface;
using SchoolHub.Models;
using SchoolHub.ViewComponents;
using System.Security.Claims;
namespace SchoolHub.ViewComponents
{
    public class SidebarViewComponent(IMenuService menuService) : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var claimsPrincipal = User as ClaimsPrincipal;


            int.TryParse(
             claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier),
              out var userId
            );


            return View(new MenuViewModel
            {
                MangerSchool= menuService.GetSchoolThatManagerBar(userId),
                TeacherSchool=menuService.GetSchoolThatTeacherBar(userId)
            });
        }
    }
}
