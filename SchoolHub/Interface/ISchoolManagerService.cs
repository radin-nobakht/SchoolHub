using SchoolHub.Dto.Manager;
using SchoolHub.Dto.School;
using SchoolHub.Entity;

namespace SchoolHub.Interface
{
    public interface ISchoolManagerService
    {
        bool AddClass(ClassDto classDto);
        (bool IsCorrect, string message) AddStudent(StudentDto student, string nationalIdNumber);
        bool DeleteClass(int classId);
        List<ClassDto> GetAllClasses(int schoolId);
        double GetClassAverage(int classId);
        ClassDto GetClassDetail(int classId);
        ManagerClassInfoDto GetManagerClassDetail(ManagerClassInfoDto managerClass);
        int? GetSchoolIdByClassId(int classId);
        List<ManagerClassStudentDto> GetStudentsDetail(int classId);
        List<ManagerClassTeacherDto> GetTeachersDetail(int classId);
        bool IsClassExist(int classId);
        bool IsManagerOfSchool(int managerUserId, int schoolId);
        bool IsManagerOfSchoolClass(int schoolId, int managerUserId);
        bool IsStudentInClass(int classId, int studentUserId);
        (int Id, bool Exist) IsUserExist(string nationalIdNumber);
        (bool success, string message) UpdateClass(ClassDto clas);    
    }
}
