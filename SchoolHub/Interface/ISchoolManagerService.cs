using SchoolHub.Dto.Manager;
using SchoolHub.Dto.School;
using SchoolHub.Dto;
using SchoolHub.Entity;

namespace SchoolHub.Interface
{
    public interface ISchoolManagerService
    {
        bool AddClass(ClassDto classDto);
        OperationResultDto AddStudent(StudentDto student, string nationalIdNumber);
        OperationResultDto AddTeacher(AddTeacherDto teacher, string nationalId);
        bool DeleteClass(int classId);
        bool DeleteStudent(int studentId);
        bool DeleteTeacher(int classId, int teacherUserId);
        List<ClassDto> GetAllClasses(int schoolId);
        double GetClassAverage(int classId);
        ClassDto GetClassDetail(int classId);
        int? GetGradeIdByClassId(int classId);
        ManagerClassInfoDto GetManagerClassDetail(ManagerClassInfoDto managerClass);
        int? GetSchoolIdByClassId(int classId);
        List<ManagerClassStudentDto> GetStudentsDetail(int classId);
        List<ManagerClassTeacherDto> GetTeachersDetail(int classId);
        bool IsClassExist(int classId);
        bool IsManagerOfSchool(int managerUserId, int schoolId);
        bool IsManagerOfSchoolClass(int schoolId, int managerUserId);
        bool IsStudentInClass(int classId, int studentUserId);
        bool IsTeacherInClass(int classId, int teacherUserId);
        (int Id, bool Exist) IsUserExist(string nationalIdNumber);
        OperationResultDto UpdateClass(ClassDto clas);    
    }
}
