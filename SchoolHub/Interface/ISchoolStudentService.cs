using SchoolHub.Dto;
using SchoolHub.Dto.Student;
using SchoolHub.Entity;

namespace SchoolHub.Interface
{
    public interface ISchoolStudentService
    {
        List<AverageScoresDto> GetAverages(int studentUserId, int classId);
        List<ScoreDto> GetScores(int studentUserId, int subjectId,int classId);
        int? GetStudentClassId(int studentUserId, int schoolId);
        StudentInfoDto GetStudentInfo(int userId, int classId, int schoolId);
        SubjectInfoDto GetSubjectInfo(SubjectInfoDto subjectInfo, int classId, int userId);
        List<GeneralItemDto> GetSubjects(int classId);
    }
}
