using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto;
using SchoolHub.Interface;

namespace SchoolHub.Controllers
{
    public class AccountController(IAccountService userService) : Controller
    {
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("SchoolPage", "School");
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserDto user)
        {


            user = userService.Register(user);
            if (user.Validation.IsCorrect == false)
            {
                ViewBag.Eror = user.Validation.Message;
                return View();
            }

            var principal = userService.CreatePrincipal(user);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
            ViewBag.Code = user.Password;
            return RedirectToAction("SchoolPage", "School");

        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(UserDto user)
        {


            user = userService.LogIn(user);
            if (user.Validation.IsCorrect == false)
            {
                ViewBag.Eror = user.Validation.Message;
                return View();
            }

            var principal = userService.CreatePrincipal(user);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
            return RedirectToAction("SchoolPage", "School");
        }


    }
}
