using SchoolHub.Dto;
using SchoolHub.Dto.School;

namespace SchoolHub.Interface
{
    public interface ISchoolService
    {
        List<SchoolInfoDto> GetClasses(int schoolId);
        SchoolDto GetSchoolById(int id);
        List<SchoolDto> GetSchools(int id);
        UserDto GetManagerById(int id);
    }
}