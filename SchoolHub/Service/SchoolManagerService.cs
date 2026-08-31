using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto.Manager;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Interface;

namespace SchoolHub.Service;

public class SchoolManagerService(MyContext db, IMapper mapper) : ISchoolManagerService
{

    public (bool IsCorrect,string message) AddStudent(StudentDto student,string nationalIdNumber)
    {
        if (!IsClassExist(student.ClassId))
            return (false, "");

        var user = IsUserExist(nationalIdNumber);
        if (!user.Exist)
            return (false, "");
        student.StudentUserId = user.Id;
        if (IsStudentInClass(student.ClassId, student.StudentUserId))
            return (false, "");

        try
        {
            db.Students.Add(mapper.Map<StudentEntity>(student));
            db.SaveChanges();
            return (true, "");
        }
        catch
        {
            return (false, "");
        }
    }


    public bool IsClassExist(int classId) => db.Classes.Any(x => x.Id == classId);

    public (int Id, bool Exist) IsUserExist(string nationalIdNumber)
    {
        var user = db.Users.FirstOrDefault(x => x.NationalIdNumber == nationalIdNumber);
        return (user?.Id ?? 0, user != null);
    }

    public bool IsStudentInClass(int classId, int studentUserId) => db.Students.Any(x => x.ClassId == classId && x.StudentUserId == studentUserId);
  

    public bool IsManagerOfSchool(int managerUserId, int schoolId)
    {
        return db.Schools.Any(x => x.Id == schoolId && x.ManagerUserId == managerUserId);
    }

    public List<ClassDto> GetAllClasses(int schoolId)
    {
        return (
            from classes in db.Classes

            where classes.SchoolId == schoolId

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
            }
        ).ToList();
    }

    public bool AddClass(ClassDto classDto)
    {
        if (string.IsNullOrWhiteSpace(classDto?.Name))
            return false;

        if (classDto.GradeGeneralId == 0 || classDto.SchoolId == 0)
        {
            return false;
        }

        var isDuplicate = db.Classes.Any(x =>
            x.GradeGeneralId == classDto.GradeGeneralId
            && x.Name == classDto.Name
            && x.SchoolId == classDto.SchoolId
            && x.MajorGeneralId == classDto.MajorGeneralId
        );

        if (isDuplicate)
            return false;

        db.Classes.Add(mapper.Map<ClassEntity>(classDto));

        db.SaveChanges();

        return true;
    }

    public bool DeleteClass(int classId)
    {
        var entity = db.Classes.FirstOrDefault(x => x.Id == classId);

        try
        {
            db.Classes.Remove(entity);
            db.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsManagerOfSchoolClass(int schoolId, int managerUserId) =>
        db.Schools.Any(x => x.Id == schoolId && x.ManagerUserId == managerUserId);

    public int? GetSchoolIdByClassId(int classId) =>
        db.Classes.FirstOrDefault(x => x.Id == classId)?.SchoolId;

    //
    public ManagerClassInfoDto GetManagerClassDetail(ManagerClassInfoDto managerClass)
    {
        var clas = db.Classes.FirstOrDefault(x => x.Id == managerClass.ClassId);
        managerClass.SchoolName = db.Schools.FirstOrDefault(x => x.Id == managerClass.SchoolId)?.Name?? "نامشخص";
        managerClass.ClassName= clas?.Name?? "نامشخص";
        managerClass.MajorName = db.GeneralItems.FirstOrDefault(x => x.Id == (clas != null ? clas.MajorGeneralId : 0))?.Title;
        managerClass.GradeName = db.GeneralItems.FirstOrDefault(x => x.Id == (clas != null ? clas.GradeGeneralId : 0))?.Title ?? "نامشخص";
        

        return managerClass;
    }

    public double GetClassAverage(int classId)
    {
        var studentIds = db.Students.Where(x => x.ClassId == classId).ToList().Select(x=> x.Id)?? [];
        var scores =db.Scores.Where(x=> studentIds.Contains(x.StudentId) && x.Status == true).ToList();
        return Math.Round(scores?.Select(x => (double?)double.Parse(x.Score)).Average() ?? 0, 2);
    }


    public ClassDto GetClassDetail(int classId) => mapper.Map<ClassDto>(db.Classes.FirstOrDefault(x=> x.Id == classId));

    public List<ManagerClassStudentDto> GetStudentsDetail(int classId)
    {
        var students = db.Students
            .Where(s => s.ClassId == classId)
            .Select(s => new
            {
                StudentId = s.Id,

                FullName = db.Users
                    .Where(u => u.Id == s.StudentUserId)
                    .Select(u => u.Name + " " + u.LastName)
                    .FirstOrDefault() ?? "نامشخص",

                Gender = db.Users
                    .Where(u => u.Id == s.StudentUserId)
                    .Join(
                        db.GeneralItems,
                        u => u.GenderGeneralId,
                        g => g.Id,
                        (u, g) => g.Title
                    )
                    .FirstOrDefault(),
                NationalIdNumber= db.Users.Where(u => u.Id == s.StudentUserId).Select(u => u.NationalIdNumber).FirstOrDefault() ?? "0"
            })
            .ToList();

        var studentIds = students
            .Select(x => x.StudentId)
            .ToList();

        var averages = db.Scores
            .Where(x => studentIds.Contains(x.StudentId)&& x.Status == true)
            .AsEnumerable()
            .GroupBy(x => x.StudentId)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(
                    g.Select(x => double.Parse(x.Score))
                     .DefaultIfEmpty(0)
                     .Average(),
                    2
                )
            );

        return students
            .Select(s => new ManagerClassStudentDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Gender = s.Gender,
                NationalIdNumber = s.NationalIdNumber,
                Average = averages.TryGetValue(s.StudentId, out var average)
                    ? average
                    : 0
            })
            .ToList();
    }
   
    public List<ManagerClassTeacherDto> GetTeachersDetail(int classId)
    {
        var assignments = db.TeachingAssignments
            .Where(x => x.ClassId == classId)
            .Select(x => new
            {
                x.TeacherUserId
            })
            .ToList();

        var teacherIds = assignments
            .Select(x => x.TeacherUserId)
            .Distinct()
            .ToList();

        var teachers = db.Users
            .Where(x => teacherIds.Contains(x.Id))
            .Select(x => new
            {
                TeacherUserId = x.Id,
                FullName = x.Name + " " + x.LastName,
                NationalIdNumber = x.NationalIdNumber
            })
            .ToList();

        var subjects = db.TeachingAssignments
            .Where(x => teacherIds.Contains(x.TeacherUserId))
            .Join(
                db.GeneralItems,
                x => x.SubjectId,
                g => g.Id,
                (x, g) => new
                {
                    x.TeacherUserId,
                    Subject = new GeneralItemDto
                    {
                        Id = g.Id,
                        Title = g.Title
                    }
                }
            )
            .ToList();

        return teachers
            .Select(teacher => new ManagerClassTeacherDto
            {
                TeacherUserId = teacher.TeacherUserId,
                FullName = teacher.FullName,
                NationalIdNumber = teacher.NationalIdNumber,
                Subjects = subjects
                    .Where(x => x.TeacherUserId == teacher.TeacherUserId)
                    .Select(x => x.Subject)
                    .DistinctBy(x => x.Id)
                    .ToList()
            })
            .ToList();
    }


    public (bool success,string message) UpdateClass(ClassDto clas)
    {
        var isEnable = db.Classes.Any(x=> x.Id ==clas.Id);
        if (!isEnable)
        {
            return (false,"کلاسی یافت نشد");
        }
        else
        {
            try
            {
                db.Classes.Update(mapper.Map<ClassEntity>(clas));
                db.SaveChanges();
                return (true,"ویرایش موفقیت میز بود");
            }
            catch
            {
                return (true,"ویرایش با شکست مواجه شد");
            }
        }

    }

}
