using AutoMapper;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Dto.Student;
using SchoolHub.Entity;
using SchoolHub.Interface;

namespace SchoolHub.Service
{
    public class StudentService(IMapper mapper, MyContext db) : IStudentService
    {
        public int? GetStudentClassId(int studentUserId,int schoolId)
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

        public List<ScoreDto> GetScores(int studentUserId, int subjectId)
        {
            var scores = db.Scores.Where(x => x.StudentId == studentUserId && x.GeneralSubjectId == subjectId).ToList();

            return mapper.Map<List<ScoreDto>>(scores);

        }

        public List<GeneralItemDto> GetSubjects(int classId)
        {
            var gradeId = db.Classes.FirstOrDefault(x => x.Id == classId).GradeGeneralId;

            var lessonIds = db.GradeSubjects.Where(x => x.GradeId == gradeId).Select(x => x.SubjectId).ToList();

            var lessons = db.GeneralItems.Where(x => lessonIds.Contains(x.Id)).ToList();
            return mapper.Map<List<GeneralItemDto>>(lessons);
        }

        public SubjectInfoDto GetSubjectInfo(SubjectInfoDto subjectInfo, int classId, int userId)
        {
            subjectInfo.Name = db.GeneralItems.FirstOrDefault(x => x.Id == subjectInfo.SubjectId && x.TitleType == "Subject").Title;

            var teacher = db.Users.FirstOrDefault(x => x.Id == db.TeachingAssignments.FirstOrDefault(z => z.SubjectId == subjectInfo.SubjectId && z.ClassId == classId).TeacherUserId);

            subjectInfo.TeacherName = teacher != null ? teacher.Name + " " + teacher.LastName : "ناشناس";
            
            subjectInfo.ScoreCount = subjectInfo.Scores.Count();
            subjectInfo.AverageScore = subjectInfo.ScoreCount != 0 ? subjectInfo.Scores.Average(x => x.Score):0.00;

            var student = db.Students.FirstOrDefault(x => x.StudentUserId == userId);
            subjectInfo.PositiveCount = student.Positives ?? 0;
            subjectInfo.NegativeCount = student.Negatives ?? 0;

            return subjectInfo;
        }

        public StudentInfoDto GetStudentInfo(int userId, int classId, int schoolId)
        {
            var studentInfo = new StudentInfoDto();

            var user = db.Users.FirstOrDefault(x => x.Id == userId);
            var clas = db.Classes.FirstOrDefault(x => x.Id == classId);
            var major = db.GeneralItems.FirstOrDefault(x => x.Id == (clas.MajorGeneralId ?? 0));


            studentInfo.FullName = user.Name + " " + user.LastName;
            studentInfo.Grade = db.GeneralItems.FirstOrDefault(x => x.Id == clas.GradeGeneralId).Title;
            studentInfo.Major = major != null ? major.Title : null;
            studentInfo.Gender = db.GeneralItems.FirstOrDefault(x => x.Id == user.GenderGeneralId).Title;
            studentInfo.SchoolName = db.Schools.FirstOrDefault(x => x.Id == schoolId).Name;
            return studentInfo;
        }

    }
}
