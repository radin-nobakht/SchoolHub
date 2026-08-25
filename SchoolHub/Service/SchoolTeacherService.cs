using AutoMapper;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Dto.Teacher;
using SchoolHub.Interface;

namespace SchoolHub.Service
{
    public class SchoolTeacherService(MyContext db, IMapper mapper) : ISchoolTeacherService
    {
        public List<ClassDto> GetTeacherClasses(int schoolId, int teacherUserId)
        {
            var result =
                from assignment in db.TeachingAssignments

                join classes in db.Classes on assignment.ClassId equals classes.Id

                where assignment.TeacherUserId == teacherUserId && classes.SchoolId == schoolId

                join major in db.GeneralItems.Where(x => x.TitleType == "Major")
                    on classes.MajorGeneralId equals major.Id
                    into majorGroup
                from major in majorGroup.DefaultIfEmpty()

                join grade in db.GeneralItems.Where(x => x.TitleType == "Grade")
                    on classes.GradeGeneralId equals grade.Id

                select new ClassDto
                {
                    Id = classes.Id,

                    Name = classes.Name,

                    Major = major != null ? major.Title : null,

                    Grade = grade.Title,
                };

            return result.Distinct().ToList();
        }

        public bool IsTeacherOfschool(int teacherUserId, int schoolId) =>
            db.TeachingAssignments.Any(x =>
                x.TeacherUserId == teacherUserId
                && db.Classes.Any(z => z.Id == x.ClassId && z.SchoolId == schoolId)
            );

        public int GetSchoolIdByClassId(int classId) =>
            db.Classes.FirstOrDefault(x => x.Id == classId).SchoolId;

        public bool IsTeacherOfClass(int teacherUserId, int classId) =>
            db.TeachingAssignments.Any(x =>
                x.TeacherUserId == teacherUserId && x.ClassId == classId
            );

        public string GetSchoolName(int schoolId) =>
            db.Schools.FirstOrDefault(x => x.Id == schoolId).Name;

        public TeacherClassInfoDto GetClassDetail(int classId)
        {
            var clas = db.Classes.FirstOrDefault(x => x.Id == classId);
            var major = db.GeneralItems.FirstOrDefault(x => x.Id == clas.MajorGeneralId);
            return new TeacherClassInfoDto
            {
                ClassName = clas.Name,
                Grade = db.GeneralItems.FirstOrDefault(x => x.Id == clas.GradeGeneralId).Title,
                Major = major?.Title,
            };
        }

        public List<TeacherClassStudentDto> GetStudentsDetail(int classId)
        {
            return (
                from student in db.Students
                where student.ClassId == classId

                join user in db.Users on student.StudentUserId equals user.Id
                select new TeacherClassStudentDto
                {
                    StudentId = student.StudentUserId,

                    FullName = user.Name + " " + user.LastName,

                    Positives = student.Positives ?? 0,

                    Negatives = student.Negatives ?? 0,

                    FirstTermGPA = (double)(student.FirstTermGPA ?? 0m),

                    SecondTermGPA = (double)(student.SecondTermGPA ?? 0m),
                }
            ).ToList();
        }

        public List<TeacherClassSubjectDto> GetSubjectDetail(int teacherId, int classId)
        {
            var subjects = (
                from assignment in db.TeachingAssignments

                where assignment.TeacherUserId == teacherId && assignment.ClassId == classId

                join subject in db.GeneralItems on assignment.SubjectId equals subject.Id

                select new TeacherClassSubjectDto { SubjectId = subject.Id, Name = subject.Title }
            ).ToList();

            return subjects;
        }
    }
}
