using SchoolHub.Dto;
using System.Security.Claims;

namespace SchoolHub.Interface
{
    public interface IUserService
    {
        ClaimsPrincipal CreatePrincipal(UserDto userDto);
        UserDto LogIn(UserDto userDto);
        UserDto Register(UserDto userDto);
    }
}