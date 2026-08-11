using SchoolHub.Dto;
using SchoolHub.Dto.School;

namespace SchoolHub.Interface
{
    public interface ISchoolService
    {
        bool AddClass(ClassDto classDto);
        bool AddSchool(SchoolDto school);
        bool DeleteClass(int id);
        ClassDto GetClassById(int classId);
        List<ClassDto> GetClasses(int schoolId);
        UserDto GetManagerById(int id);
        SchoolDto GetSchoolById(int id);
        List<SchoolDto> GetSchools(int id);
        List<StudentDto> GetStudentByClassId(int classId);
        List<TeacherDto> GetTeacherByClassId(int classId);
    }
}