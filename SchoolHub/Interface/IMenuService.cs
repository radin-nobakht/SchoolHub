using SchoolHub.Dto.School;

namespace SchoolHub.Interface
{
    public interface IMenuService
    {
        List<SchoolBarDto> GetSchoolThatManagerBar(int userId);
        List<SchoolBarDto> GetSchoolThatStudentrBar(int userId);
        List<SchoolBarDto> GetSchoolThatTeacherBar(int userId);
    }
}
