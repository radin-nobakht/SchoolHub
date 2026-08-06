using SchoolHub.Dto.School;

namespace SchoolHub.Interface
{
    public interface IMenuService
    {
        List<SchoolBarDto> GetSchoolBar(int userId);
    }
}