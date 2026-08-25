using SchoolHub.Dto.School;
using SchoolHub.Dto.Teacher;

namespace SchoolHub.Interface
{
    public interface ISchoolTeacherService
    {
        TeacherClassInfoDto GetClassDetail(int classId);
        int GetSchoolIdByClassId(int classId);
        string GetSchoolName(int schoolId);
        List<TeacherClassStudentDto> GetStudentsDetail(int classId);
        List<TeacherClassSubjectDto> GetSubjectDetail(int teacherId, int classId);
        List<ClassDto> GetTeacherClasses(int schoolId, int teacherUserId);
        bool IsTeacherOfClass(int teacherUserId, int classId);
        bool IsTeacherOfschool(int teacherUserId, int schoolId);
    }
}
