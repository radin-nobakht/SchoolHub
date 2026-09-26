using SchoolHub.Dto;
using SchoolHub.Dto.School;
using SchoolHub.Models;

namespace SchoolHub.Interface
{
    public interface ISchoolService
    {
        ClassDto? GetClassById(int classId);

        //List<TeacherDto> GetTeacherByClassId(int classId);

        List<StudentDto> GetStudentByClassId(int classId);

        List<int> GetTeacherSchoolIds(int userId);

        List<int> GetManagerSchoolIds(int userId);

        List<int> GetStudentSchoolIds(int userId);

        List<SchoolListItemViewModel> GetSchools(int userId);

        SchoolDto? GetSchoolById(int id);

        SchoolDto? GetManagerSchoolById(int schoolId, int managerUserId);

        UserDto? GetManagerById(int id);

        bool AddSchool(SchoolDto school);

        //List<TeacherDto> GetMySubjectsByClassId(int classId, int teacherUserId);
    }
}
