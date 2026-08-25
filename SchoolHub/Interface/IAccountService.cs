using System.Security.Claims;
using SchoolHub.Dto;

namespace SchoolHub.Interface
{
    public interface IAccountService
    {
        ClaimsPrincipal CreatePrincipal(UserDto userDto);
        UserDto LogIn(UserDto userDto);
        UserDto Register(UserDto userDto);
    }
}
