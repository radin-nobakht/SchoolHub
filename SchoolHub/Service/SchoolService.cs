using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Interface;
using SchoolHub.Models;

namespace SchoolHub.Service;

public class SchoolService(MyContext db, IMapper mapper) : ISchoolService
{
    public ClassDto? GetClassById(int classId)
    {
        return (
            from clas in db.Classes

            where clas.Id == classId

            join generalGrade in db.GeneralItems.Where(x => x.TitleType == "Grade")
                on clas.GradeGeneralId equals generalGrade.Id

            join generalMajor in db.GeneralItems.Where(x => x.TitleType == "Major")
                on clas.MajorGeneralId equals generalMajor.Id
                into majorGroup
            from generalMajor in majorGroup.DefaultIfEmpty()

            select new ClassDto
            {
                Id = clas.Id,

                Name = clas.Name,

                Grade = generalGrade.Title,

                Major = generalMajor != null ? generalMajor.Title : null,
            }
        ).FirstOrDefault();
    }

    //public List<TeacherDto> GetTeacherByClassId(int classId)
    //{
    //    return (
    //        from teacherData in db.TeachingAssignments

    //        where teacherData.ClassId == classId

    //        join teacherUser in db.Users on teacherData.TeacherUserId equals teacherUser.Id

    //        join generalSubject in db.GeneralItems.Where(x => x.TitleType == "Subject")
    //            on teacherData.SubjectId equals generalSubject.Id

    //        select new TeacherDto
    //        {
    //            TeacherName = teacherUser.Name + " " + teacherUser.LastName,

    //            SubjectName = generalSubject.Title,
    //        }
    //    ).ToList();
    //}

    public List<StudentDto> GetStudentByClassId(int classId)
    {
        return (
            from student in db.Students

            where student.ClassId == classId && student.IsDeleted== false

            join studentUser in db.Users on student.StudentUserId equals studentUser.Id

            select new StudentDto
            {
                Id = student.Id,

                StudentFullName = studentUser.Name + " " + studentUser.LastName,

                Positives = student.Positives ?? 0,

                Negatives = student.Negatives ?? 0,

                FirstTermGPA = student.FirstTermGPA ?? 0.00m,

                SecondTermGPA = student.SecondTermGPA ?? 0.00m,
            }
        ).ToList();
    }

    public List<int> GetTeacherSchoolIds(int userId)
    {
        return (
            from assignment in db.TeachingAssignments

            join clas in db.Classes on assignment.ClassId equals clas.Id

            where assignment.TeacherUserId == userId

            select clas.SchoolId
        )
            .Distinct()
            .ToList();
    }

    public List<int> GetStudentSchoolIds(int userId)
    {
        return (
            from student in db.Students

            where student.StudentUserId == userId && student.IsDeleted== false

            join clas in db.Classes on student.ClassId equals clas.Id

            select clas.SchoolId
        )
            .Distinct()
            .ToList();
    }

    public List<int> GetManagerSchoolIds(int userId)
    {
        return db.Schools
            .Where(x => x.ManagerUserId == userId)
            .Select(x => x.Id)
            .ToList();
    }

    public List<SchoolListItemViewModel> GetSchools(int userId)
    {
        var schoolIds = GetManagerSchoolIds(userId)
            .Union(GetTeacherSchoolIds(userId))
            .Union(GetStudentSchoolIds(userId))
            .ToList();

        var schools =
            from school in db.Schools

            join city in db.GeneralItems
                on school.CityId equals city.Id

            join province in db.GeneralItems
                on city.ParentId equals province.Id

            // District اختیاری است
            join district in db.GeneralItems
                on school.DistrictId equals district.Id into districtGroup

            from district in districtGroup.DefaultIfEmpty()

            where schoolIds.Contains(school.Id)

            select new SchoolListItemViewModel
            {
                School = new SchoolDto
                {
                    Id = school.Id,

                    Name = school.Name,

                    ManagerUserId = school.ManagerUserId,

                    Province = province.Title,

                    City = city.Title,

                    District = district != null
                        ? district.Title
                        : "منطقه 1"
                },

                IsManager = school.ManagerUserId == userId,

                IsTeacher = db.TeachingAssignments.Any(assignment =>
                    assignment.TeacherUserId == userId
                    && db.Classes.Any(clas =>
                        clas.Id == assignment.ClassId
                        && clas.SchoolId == school.Id
                    )
                ),

                IsStudent = db.Students.Any(student =>
                    student.StudentUserId == userId
                    && !student.IsDeleted
                    && db.Classes.Any(clas =>
                        clas.Id == student.ClassId
                        && clas.SchoolId == school.Id
                    )
                )
            };

        return schools.ToList();
    }
    public SchoolDto? GetSchoolById(int id)
    {
        var school = db.Schools.FirstOrDefault(x => x.Id == id);

        return school == null ? null : mapper.Map<SchoolDto>(school);
    }

    public SchoolDto? GetManagerSchoolById(int schoolId, int managerUserId)
    {
        var school = db.Schools.FirstOrDefault(x =>
            x.Id == schoolId && x.ManagerUserId == managerUserId
        );

        return school == null ? null : mapper.Map<SchoolDto>(school);
    }

    public UserDto? GetManagerById(int id)
    {
        var user = db.Users.FirstOrDefault(x => x.Id == id);

        return user == null ? null : mapper.Map<UserDto>(user);
    }

    public bool AddSchool(SchoolDto school)
    {
        if (string.IsNullOrWhiteSpace(school.Name))
            return false;

        if (
            school.CityId == 0
            || school.GenderGeneralId == 0
            || school.TypeGeneralId == 0
            || school.ShiftGeneralId == 0
            || school.EducationLevelGeneralId == 0
            || school.EducationPeriodGeneralId == 0
        )
        {
            return false;
        }

        var isDuplicate = db.Schools.Any(x =>
            x.Name == school.Name && x.ShiftGeneralId == school.ShiftGeneralId
        );

        if (isDuplicate)
            return false;

        db.Schools.Add(mapper.Map<SchoolEntity>(school));

        db.SaveChanges();

        return true;
    }

    //public List<TeacherDto> GetMySubjectsByClassId(int classId, int teacherUserId)
    //{
    //    var result =
    //        from assignment in db.TeachingAssignments

    //        where assignment.ClassId == classId && assignment.TeacherUserId == teacherUserId

    //        join teacher in db.Users on assignment.TeacherUserId equals teacher.Id

    //        join subject in db.GeneralItems.Where(x => x.TitleType == "Subject")
    //            on assignment.SubjectId equals subject.Id

    //        select new TeacherDto
    //        {
    //            TeacherName = teacher.Name + " " + teacher.LastName,

    //            SubjectName = subject.Title,
    //        };

    //    return result.ToList();
    //}
}
