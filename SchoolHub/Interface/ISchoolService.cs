using SchoolHub.Dto;
using SchoolHub.Dto.School;
using SchoolHub.Models;

namespace SchoolHub.Interface
{
    public interface ISchoolService
    {
        List<TeacherDto> GetMySubjectsByClassId(int classId,int teacherUserId);
        List<ClassDto> GetTeacherClasses(int schoolId, int teacherUserId);
        bool IsTeacherAssignedToClass(int teacherUserId, int classId);
        bool AddClass(ClassDto classDto);
        bool AddSchool(SchoolDto school);
        bool DeleteClass(int id);
        ClassDto GetClassById(int classId);
        List<ClassDto> GetClasses(int schoolId);
        UserDto GetManagerById(int id);
        SchoolDto GetSchoolById(int id);
        List<SchoolListItemViewModel> GetSchools(int userId);
        List<StudentDto> GetStudentByClassId(int classId);
        List<TeacherDto> GetTeacherByClassId(int classId);

    }
}