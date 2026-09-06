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
        bool IsSubjectExistForThisGrade(List<int> subjectIds, int GradeId);
        List<GeneralItemDto> GetAvailableSubjectsByGradeId(int gradeId, int classId);
        List<GeneralItemDto> GetTeacherSubjects(int classId, int teacherUserId);
        List<GeneralItemDto> GetAvailableSubjectsByGradeIdForUpdateTeacher(int gradeId, int classId, int teacherUserId);
    }
}
