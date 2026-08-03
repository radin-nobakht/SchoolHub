using SchoolHub.Dto;
using SchoolHub.Dto.School;
using SchoolHub.Entity;

namespace SchoolHub.Interface
{
    public interface ISchoolService
    {
        bool AddClass(ClassDto classDto);
        bool AddSchool(SchoolDto school);
        bool DeleteClass(int id);
        List<GeneralItemDto> GetCitiesByProvinceId(int provinceId);
        List<ClassDto> GetClasses(int schoolId);
        List<GeneralItemDto> GetDistrictByCityId(int cityId);
        UserDto GetManagerById(int id);
        Dictionary<string, List<GeneralItemDto>> GetGeneralItems(string[] titles);
        SchoolDto GetSchoolById(int id);
        List<SchoolDto> GetSchools(int id);
    }
}