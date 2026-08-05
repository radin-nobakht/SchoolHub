using SchoolHub.Dto.School;

namespace SchoolHub.Service
{
    public interface IMenuService
    {
        List<SchoolBarDto> GetSchoolBar(int userId);
    }
}