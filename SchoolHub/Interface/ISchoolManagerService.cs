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
        OperationResultDto AddTeacher(Dto.Manager.TeacherDto teacher, string nationalId);
        bool DeleteClass(int classId);
        bool DeleteStudent(int studentId);
        bool DeleteTeacher(int classId, int teacherUserId);
        List<ClassDto> GetAllClasses(int schoolId);
        double GetClassAverage(int classId);
        ClassDto GetClassDetail(int classId);
        StudentReportCardDto GetClassDetailForManager(int studentId);
        int GetClassIdByStudentAndSchoolId(int studentUserId, int schoolId);
        int? GetGradeIdByClassId(int classId);
        ManagerClassInfoDto GetManagerClassDetail(ManagerClassInfoDto managerClass);
        double GetOverallAverage(int studentId);
        int? GetSchoolIdByClassId(int classId);
        List<ReportCardSubjectDto> GetSubjectsDetailForManager(int studentId, int classId);
        int GetStudentIdByStudentAndClassId(int studentUserId, int classId);
        List<ManagerClassStudentDto> GetStudentsDetail(int classId);
        List<ManagerClassTeacherDto> GetTeachersDetail(int classId);
        bool IsClassExist(int classId);
        bool IsManagerOfSchool(int managerUserId, int schoolId);
        bool IsManagerOfSchoolClass(int schoolId, int managerUserId);
        bool IsStudentInClass(int classId, int studentUserId);
        bool IsStudentInSchool(int classId, int studentUserId);
        bool IsTeacherInClass(int classId, int teacherUserId);
        bool IsUserExistById(int userId);
        (int Id, bool Exist) IsUserExistByNationalId(string nationalIdNumber);
        OperationResultDto UpdateClass(ClassDto clas);
        OperationResultDto UpdateTeacher(Dto.Manager.TeacherDto teacher);
        bool IsSchoolExist(int schoolId);
        string GetNationalId(int userId);
        UserEntity GetUserByNationalId(string nationalId);
        ValidationDto UpdateSchool(SchoolDto schoolDto);
    }
}
