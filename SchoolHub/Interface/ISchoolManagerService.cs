using SchoolHub.Dto.School;

namespace SchoolHub.Interface
{
    public interface ISchoolManagerService
    {
        bool AddClass(ClassDto classDto);
        bool DeleteClass(int classId);
        List<ClassDto> GetAllClasses(int schoolId);
        int? GetSchoolIdByClassId(int classId);
        bool IsManagerOfSchool(int managerUserId, int schoolId);
        bool IsManagerOfSchoolClass(int schoolId, int managerUserId);
    }
}
