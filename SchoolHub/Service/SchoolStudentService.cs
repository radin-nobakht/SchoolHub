using AutoMapper;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Dto.Student;
using SchoolHub.Entity;
using SchoolHub.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolHub.Service
{
    public class SchoolStudentService(IMapper mapper, MyContext db) : ISchoolStudentService
    {
        public int? GetStudentClassId(int studentUserId, int schoolId)
        {
            return (
                from student in db.Students
                where student.StudentUserId == studentUserId

                join clas in db.Classes
                on student.ClassId equals clas.Id
                where clas.SchoolId == schoolId
                select clas.Id
                ).FirstOrDefault();
        }


        public List<AverageScoresDto> GetAverages(int studentUserId, int classId)
        {
            var scoreAverage = new List<AverageScoresDto>();
            var gradeId = db.Classes.FirstOrDefault(x => x.Id == classId)?.GradeGeneralId ?? 0;
            var subjectIds = db.GradeSubjects.Where(x => x.GradeId == gradeId).Select(x => x.SubjectId).ToList();
            var studentId = db.Students.FirstOrDefault(x => x.StudentUserId == studentUserId && x.ClassId == classId)?.Id ?? 0;
            var scores = db.Scores.Where(x => x.StudentId == studentId && x.Status == true).ToList();
            var a = db.GeneralItems.Where(x => subjectIds.Contains(x.Id)).ToList();
            foreach (var s in a)
            {
                var subjectScores = scores.Where(x => x.GeneralSubjectId == s.Id).ToList();
                scoreAverage.Add(new AverageScoresDto
                {
                    SubjectId = s.Id,
                    AverageScores = subjectScores.Count() > 0 ?subjectScores.Average(x => double.Parse(x.Score)): 0.00
                });
            }
            return scoreAverage;
        }


        public List<ScoreDto> GetScores(int studentUserId, int subjectId,int classId)
        {
            var studentId = db.Students.FirstOrDefault(x => x.ClassId == classId && x.StudentUserId == studentUserId)?.Id;
            var scores = db.Scores.Where(x => x.StudentId == studentId && x.GeneralSubjectId == subjectId && x.Status == true).ToList();

            return mapper.Map<List<ScoreDto>>(scores);

        }

        public List<GeneralItemDto> GetSubjects(int classId)
        {
            var gradeId = db.Classes.FirstOrDefault(x => x.Id == classId)?.GradeGeneralId ?? 0;

            var lessonIds = db.GradeSubjects.Where(x => x.GradeId == gradeId).Select(x => x.SubjectId).ToList();

            var lessons = db.GeneralItems.Where(x => lessonIds.Contains(x.Id)).ToList();
            return mapper.Map<List<GeneralItemDto>>(lessons);
        }

        public SubjectInfoDto GetSubjectInfo(SubjectInfoDto subjectInfo, int classId, int userId)
        {
            subjectInfo.Name = db.GeneralItems.FirstOrDefault(x => x.Id == subjectInfo.SubjectId && x.TitleType == "Subject")?.Title ?? "نامشخص";
            var teachingAssistant = db.TeachingAssignments.FirstOrDefault(z => z.SubjectId == subjectInfo.SubjectId && z.ClassId == classId)?.TeacherUserId ?? 0;
            var teacher = db.Users.FirstOrDefault(x => x.Id == teachingAssistant);

            subjectInfo.TeacherName = teacher != null ? teacher.Name + " " + teacher.LastName : "ناشناس";

            subjectInfo.ScoreCount = subjectInfo.Scores.Count();
            subjectInfo.AverageScore =  Math.Round(subjectInfo.ScoreCount != 0 ? subjectInfo.Scores.Average(x => x.Score) : 0.00, 2);

            var studentId = db.Students.FirstOrDefault(x => x.StudentUserId == userId && x.ClassId == classId)?.Id ?? 0;
            var studentSubjectRecords = db.StudentSubjectRecords.Where(x => x.SubjectGeneralId == subjectInfo.SubjectId && x.StudentUserId == studentId).ToList();
            subjectInfo.PositiveCount = studentSubjectRecords.Where(x => x.Type == "Positive")?.Sum(x => x.Count) ?? 0;
            subjectInfo.NegativeCount = studentSubjectRecords.Where(x => x.Type == "Negative")?.Sum(x => x.Count) ?? 0;
            return subjectInfo;
        }

        public StudentInfoDto GetStudentInfo(int userId, int classId, int schoolId)
        {
            var studentInfo = new StudentInfoDto();

            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            var clas = db.Classes.FirstOrDefault(x => x.Id == classId);


            studentInfo.FullName = user!.Name + " " + user.LastName;
            studentInfo.Grade = db.GeneralItems.FirstOrDefault(x => x.Id == (clas != null ? clas.GradeGeneralId : 0))?.Title ?? "نامشخص";
            studentInfo.Major = db.GeneralItems.FirstOrDefault(x => x.Id == (clas != null ? clas.MajorGeneralId : 0))?.Title;
            studentInfo.Gender = db.GeneralItems.FirstOrDefault(x => x.Id == user.GenderGeneralId)?.Title ?? "نامشخص";
            studentInfo.SchoolName = db.Schools.FirstOrDefault(x => x.Id == schoolId)?.Name ?? "نامشخص";
            studentInfo.ClassId = classId;
            return studentInfo;
        }

    }
}
