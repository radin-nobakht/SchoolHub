using SchoolHub.Dto.School;
using SchoolHub.Entity;

namespace SchoolHub.Interface
{
    public interface IGeneralService
    {
        List<GeneralItemDto> GetCitiesByProvinceId(int provinceId);
        List<GeneralItemDto> GetDistrictByCityId(int cityId);
        Task<FullSchoolDataDto> GetFullDataOfSchoolById(int schoolId);
        Dictionary<string, List<GeneralItemDto>> GetGeneralItems(string[] titles);
        List<GeneralItemDto> GetGeneralGrades(int schoolId);
        List<GeneralItemDto> GetGenerals(string type,int? parentId=null);
        GeneralItemDto GetCurrentItem(int classId, string type);
    }
}
