using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Entity;
using SchoolHub.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SchoolHub.Service
{
    public class AccountService(MyContext db, IMapper mapper) : IAccountService
    {
        public UserDto Register(UserDto userDto)
        {
            userDto.Validation.IsCorrect =
                db.Users.FirstOrDefault(x =>
                    x.NationalIdNumber == userDto.NationalIdNumber
                    && x.IsStudent == userDto.IsStudent
                ) == null
                    ? true
                    : false;
            if (userDto.Validation.IsCorrect)
            {
                db.Users.Add(mapper.Map<UserEntity>(userDto));
                db.SaveChanges();
                CreatePrincipal(userDto);
            }
            else
                userDto.Validation.Message = "کد ملی شما تکراری هست";

            return userDto;
        }

        public UserDto LogIn(UserDto userDto)
        {
            var validationDto = new ValidationDto();
            var userEntity = db.Users.FirstOrDefault(x =>
                x.NationalIdNumber == userDto.NationalIdNumber && x.Password == userDto.Password
            );
            if (userEntity != null)
                userDto.Validation.IsCorrect = true;
            else
            {
                userDto.Validation.IsCorrect = false;
                userDto.Validation.Message = "کد ملی یا کد اشتباه است";
                return userDto;
            }

            return mapper.Map<UserDto>(userEntity);
        }

        public ClaimsPrincipal CreatePrincipal(UserDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.Name),
                new Claim("NationalIdNumber", userDto.NationalIdNumber),
                new Claim("Password", userDto.Password),
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return new ClaimsPrincipal(identity);
        }
    }
}
