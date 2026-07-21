using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Dto;
using SchoolHub.Interface;
using SchoolHub.Models;
using SchoolHub.Service;
using System.Diagnostics;

namespace SchoolHub.Controllers
{
    public class UserController(IUserService userService) : Controller
    {
        public IActionResult Index()
        {
            return View();
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
            ViewBag.Code = user.Code;
            return View("ShowCode");

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

        public IActionResult ShowCode(string code)
        {
            return View(code);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
