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
    public bool IsTeacherAssignedToClass(int teacherUserId, int classId)
    {
        return db.TeachingAssignments.Any(x =>
            x.TeacherUserId == teacherUserId &&
            x.ClassId == classId);
    }


    public ClassDto? GetClassById(int classId)
    {
        return
            (
                from clas in db.Classes

                where clas.Id == classId

                join generalGrade in db.GeneralItems
                        .Where(x => x.TitleType == "Grade")
                    on clas.GradeGeneralId equals generalGrade.Id

                join generalMajor in db.GeneralItems
                        .Where(x => x.TitleType == "Major")
                    on clas.MajorGeneralId equals generalMajor.Id
                    into majorGroup

                from generalMajor in majorGroup.DefaultIfEmpty()

                select new ClassDto
                {
                    Id = clas.Id,

                    Name = clas.Name,

                    Grade = generalGrade.Title,

                    Major = generalMajor != null
                        ? generalMajor.Title
                        : null
                }
            )
            .FirstOrDefault();
    }


    public List<TeacherDto> GetTeacherByClassId(int classId)
    {
        return
            (
                from teacherData in db.TeachingAssignments

                where teacherData.ClassId == classId

                join teacherUser in db.Users
                    on teacherData.TeacherUserId equals teacherUser.Id

                join generalSubject in db.GeneralItems
                        .Where(x => x.TitleType == "Subject")
                    on teacherData.SubjectId equals generalSubject.Id

                select new TeacherDto
                {
                    TeacherName =
                        teacherUser.Name + " " + teacherUser.LastName,

                    SubjectName = generalSubject.Title
                }
            )
            .ToList();
    }


    public List<StudentDto> GetStudentByClassId(int classId)
    {
        return
            (
                from student in db.Students

                where student.ClassId == classId

                join studentUser in db.Users
                    on student.StudentUserId equals studentUser.Id

                select new StudentDto
                {
                    Id = student.Id,

                    StudentFullName =
                        studentUser.Name + " " + studentUser.LastName,

                    Positives = student.Positives ?? 0,

                    Negatives = student.Negatives ?? 0,

                    FirstTermGPA =
                        student.FirstTermGPA ?? 0.00m,

                    SecondTermGPA =
                        student.SecondTermGPA ?? 0.00m
                }
            )
            .ToList();
    }


    public List<int> GetTeacherSchoolIds(int userId)
    {
        return
            (
                from assignment in db.TeachingAssignments

                join clas in db.Classes
                    on assignment.ClassId equals clas.Id

                where assignment.TeacherUserId == userId

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


    public List<int> GetStudentSchoolIds(int userId)
    {
        return
            (
                from student in db.Students

                where student.StudentUserId == userId

                join clas in db.Classes
                    on student.ClassId equals clas.Id

                select clas.SchoolId
            )
            .Distinct()
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

            join district in db.GeneralItems
                on school.DistrictId equals district.Id

            join city in db.GeneralItems
                on district.ParentId equals city.Id

            join province in db.GeneralItems
                on city.ParentId equals province.Id

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

                    District = district.Title
                },

                IsManager =
                    school.ManagerUserId == userId,

                IsTeacher =
                    db.TeachingAssignments.Any(assignment =>
                        assignment.TeacherUserId == userId &&
                        db.Classes.Any(clas =>
                            clas.Id == assignment.ClassId &&
                            clas.SchoolId == school.Id)),

                IsStudent =
                    db.Students.Any(student =>
                        student.StudentUserId == userId &&
                        db.Classes.Any(clas =>
                            clas.Id == student.ClassId &&
                            clas.SchoolId == school.Id))
            };

        return schools.ToList();
    }

    public bool IsManagerOfSchool(int managerUserId, int schoolId)
    {
        return db.Schools.Any(x =>
            x.Id == schoolId &&
            x.ManagerUserId == managerUserId);
    }

    public SchoolDto? GetSchoolById(int id)
    {
        var school = db.Schools
            .FirstOrDefault(x => x.Id == id);

        return school == null
            ? null
            : mapper.Map<SchoolDto>(school);
    }


    public SchoolDto? GetManagerSchoolById(
        int schoolId,
        int managerUserId)
    {
        var school = db.Schools
            .FirstOrDefault(x =>
                x.Id == schoolId &&
                x.ManagerUserId == managerUserId);

        return school == null
            ? null
            : mapper.Map<SchoolDto>(school);
    }


    public List<ClassDto> GetClasses(int schoolId)
    {
        return
            (
                from classes in db.Classes

                where classes.SchoolId == schoolId

                join major in db.GeneralItems
                        .Where(x => x.TitleType == "Major")
                    on classes.MajorGeneralId equals major.Id
                    into majorGroup

                from major in majorGroup.DefaultIfEmpty()

                join grade in db.GeneralItems
                        .Where(x => x.TitleType == "Grade")
                    on classes.GradeGeneralId equals grade.Id

                select new ClassDto
                {
                    Id = classes.Id,

                    Name = classes.Name,

                    Major = major != null
                        ? major.Title
                        : null,

                    Grade = grade.Title
                }
            )
            .ToList();
    }


    public List<ClassDto> GetManagerClasses(
        int schoolId,
        int managerUserId)
    {
        var isManager = db.Schools.Any(x =>
            x.Id == schoolId &&
            x.ManagerUserId == managerUserId);

        if (!isManager)
            return [];

        return GetClasses(schoolId);
    }


    public UserDto? GetManagerById(int id)
    {
        var user = db.Users
            .FirstOrDefault(x => x.Id == id);

        return user == null
            ? null
            : mapper.Map<UserDto>(user);
    }


    public bool AddSchool(SchoolDto school)
    {
        if (string.IsNullOrWhiteSpace(school.Name))
            return false;

        if (school.CityId == 0 ||
            school.GenderGeneralId == 0 ||
            school.TypeGeneralId == 0 ||
            school.ShiftGeneralId == 0 ||
            school.EducationLevelGeneralId == 0 ||
            school.EducationPeriodGeneralId == 0)
        {
            return false;
        }

        var isDuplicate = db.Schools.Any(x =>
            x.Name == school.Name &&
            x.ShiftGeneralId == school.ShiftGeneralId);

        if (isDuplicate)
            return false;

        db.Schools.Add(
            mapper.Map<SchoolEntity>(school));

        db.SaveChanges();

        return true;
    }


    public List<ClassDto> GetTeacherClasses(
        int schoolId,
        int teacherUserId)
    {
        var result =
            from assignment in db.TeachingAssignments

            join classes in db.Classes
                on assignment.ClassId equals classes.Id

            where assignment.TeacherUserId == teacherUserId
                  && classes.SchoolId == schoolId

            join major in db.GeneralItems
                    .Where(x => x.TitleType == "Major")
                on classes.MajorGeneralId equals major.Id
                into majorGroup

            from major in majorGroup.DefaultIfEmpty()

            join grade in db.GeneralItems
                    .Where(x => x.TitleType == "Grade")
                on classes.GradeGeneralId equals grade.Id

            select new ClassDto
            {
                Id = classes.Id,

                Name = classes.Name,

                Major = major != null
                    ? major.Title
                    : null,

                Grade = grade.Title
            };

        return result
            .Distinct()
            .ToList();
    }


    public bool AddClass(ClassDto classDto)
    {
        if (string.IsNullOrWhiteSpace(classDto.Name))
            return false;

        if (classDto.GradeGeneralId == 0 ||
            classDto.SchoolId == 0)
        {
            return false;
        }

        var isDuplicate = db.Classes.Any(x =>
            x.GradeGeneralId == classDto.GradeGeneralId &&
            x.Name == classDto.Name &&
            x.SchoolId == classDto.SchoolId &&
            x.MajorGeneralId == classDto.MajorGeneralId);

        if (isDuplicate)
            return false;

        db.Classes.Add(
            mapper.Map<ClassEntity>(classDto));

        db.SaveChanges();

        return true;
    }


    public bool DeleteClass(
        int classId,
        int managerUserId)
    {
        var classEntity =
            from clas in db.Classes

            join school in db.Schools
                on clas.SchoolId equals school.Id

            where clas.Id == classId &&
                  school.ManagerUserId == managerUserId

            select clas;

        var entity = classEntity.FirstOrDefault();

        if (entity == null)
            return false;

        db.Classes.Remove(entity);

        db.SaveChanges();

        return true;
    }


    public List<TeacherDto> GetMySubjectsByClassId(
        int classId,
        int teacherUserId)
    {
        var result =
            from assignment in db.TeachingAssignments

            where assignment.ClassId == classId &&
                  assignment.TeacherUserId == teacherUserId

            join teacher in db.Users
                on assignment.TeacherUserId equals teacher.Id

            join subject in db.GeneralItems
                    .Where(x => x.TitleType == "Subject")
                on assignment.SubjectId equals subject.Id

            select new TeacherDto
            {
                TeacherName =
                    teacher.Name + " " + teacher.LastName,

                SubjectName = subject.Title
            };

        return result.ToList();
    }
}