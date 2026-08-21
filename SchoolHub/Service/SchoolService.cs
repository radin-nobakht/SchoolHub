using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Entity;
using SchoolHub.Dto.School;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Interface;
using SchoolHub.Models;
using System.Net.WebSockets;

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
        var result = (
            from clas in db.Classes
            where clas.Id == classId

            join generalGrade in db.GeneralItems.Where(x => x.TitleType == "Grade")
                on clas.GradeGeneralId equals generalGrade.Id

            join generalMajor in db.GeneralItems.Where(x => x.TitleType == "Major")
                on clas.MajorGeneralId equals generalMajor.Id into majorGroup

            from generalMajor in majorGroup.DefaultIfEmpty()

            select new ClassDto
            {
                Name = clas.Name,
                Grade = generalGrade.Title,
                Major = generalMajor != null
                    ? generalMajor.Title
                    : null
            }
        ).FirstOrDefault();

        return result;
    }
    public List<TeacherDto> GetTeacherByClassId(int classId)
    {
        var result = (
            from teacherData in db.TeachingAssignments
            where teacherData.ClassId == classId

            join teacherUser in db.Users
            on teacherData.TeacherUserId equals teacherUser.Id

            join generalSubject in db.GeneralItems.Where(x => x.TitleType == "Subject")
            on teacherData.SubjectId equals generalSubject.Id

            select new TeacherDto
            {
                TeacherName = teacherUser.Name + " " + teacherUser.LastName,
                SubjectName = generalSubject.Title
            }

            ).ToList();

        return result;
    }
    public List<StudentDto> GetStudentByClassId(int classId)
    {
        var result =
            (
            from student in db.Students
            where student.ClassId == classId

            join studentUser in db.Users
            on student.StudentUserId equals studentUser.Id

            select new StudentDto
            {
                Id = student.Id,
                StudentFullName = studentUser.Name + " " + studentUser.LastName,
                Positives = student.Positives != null ? student.Positives : 0,
                Negatives = student.Negatives != null ? student.Negatives : 0,
                FirstTermGPA = student.FirstTermGPA != null ? student.FirstTermGPA : 0.00m,
                SecondTermGPA = student.SecondTermGPA != null ? student.FirstTermGPA : 0.00m
            }).ToList();
        return result;
    }

    public List<SchoolListItemViewModel> GetSchools(int userId)
    {
        var managerSchoolIds = db.Schools
            .Where(x => x.ManagerUserId == userId)
            .Select(x => x.Id);

        var teacherSchoolIds =
            from assignment in db.TeachingAssignments
            join clas in db.Classes
                on assignment.ClassId equals clas.Id
            where assignment.TeacherUserId == userId
            select clas.SchoolId;

        var schoolIds = managerSchoolIds
            .Union(teacherSchoolIds);

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

                IsManager = school.ManagerUserId == userId,

                IsTeacher = db.TeachingAssignments.Any(assignment =>
                    assignment.TeacherUserId == userId &&
                    db.Classes.Any(clas =>
                        clas.Id == assignment.ClassId &&
                        clas.SchoolId == school.Id))
            };

        return schools.ToList();
    }
    public SchoolDto GetSchoolById(int id)
    {
        return mapper.Map<SchoolDto>(db.Schools.FirstOrDefault(x => x.Id == id));
    }

    public List<ClassDto> GetClasses(int schoolId)
    {
        var result = (
            from classes in db.Classes
            where classes.SchoolId == schoolId

            join major in db.GeneralItems.Where(x => x.TitleType == "Major")
                on classes.MajorGeneralId equals major.Id into majorGroup

            from major in majorGroup.DefaultIfEmpty()

            join grade in db.GeneralItems.Where(x => x.TitleType == "Grade")
                on classes.GradeGeneralId equals grade.Id

            select new ClassDto
            {
                Id = classes.Id,
                Name = classes.Name,
                Major = major != null ? major.Title : null,
                Grade = grade.Title
            }
        ).ToList();

        return result;
    }
    public UserDto GetManagerById(int id)
    {
        return mapper.Map<UserDto>(db.Users.FirstOrDefault(x => x.Id == id));
    }

    public bool AddSchool(SchoolDto school)
    {
        if (school.Name != null && school.CityId != 0 && school.GenderGeneralId != 0 && school.TypeGeneralId != 0 && school.ShiftGeneralId != 0 && school.EducationLevelGeneralId != 0 && school.EducationPeriodGeneralId != 0)
        {
            var isDuplicate = db.Schools.FirstOrDefault(x => x.Name == school.Name && x.ShiftGeneralId == school.ShiftGeneralId) == null ? false : true;
            if (!isDuplicate)
            {

                db.Schools.Add(mapper.Map<SchoolEntity>(school));
                db.SaveChanges();
                return true;
            }
        }
        return false;
    }
    public List<ClassDto> GetTeacherClasses(int schoolId, int teacherUserId)
    {
        var result =
            from assignment in db.TeachingAssignments

            join classes in db.Classes
                on assignment.ClassId equals classes.Id

            where assignment.TeacherUserId == teacherUserId
                  && classes.SchoolId == schoolId

            join major in db.GeneralItems.Where(x => x.TitleType == "Major")
                on classes.MajorGeneralId equals major.Id into majorGroup

            from major in majorGroup.DefaultIfEmpty()

            join grade in db.GeneralItems.Where(x => x.TitleType == "Grade")
                on classes.GradeGeneralId equals grade.Id

            select new ClassDto
            {
                Id = classes.Id,
                Name = classes.Name,
                Major = major != null ? major.Title : null,
                Grade = grade.Title
            };

        return result.Distinct().ToList();
    }
    public bool AddClass(ClassDto classDto)
    {
        if (classDto.Name != null && classDto.GradeGeneralId != 0)
        {

            var isDuplicate = db.Classes.FirstOrDefault(x => x.GradeGeneralId == classDto.GradeGeneralId && x.Name == classDto.Name && x.SchoolId == classDto.SchoolId && x.MajorGeneralId == classDto.MajorGeneralId) == null ? false : true;
            if (!isDuplicate)
            {
                db.Add(mapper.Map<ClassEntity>(classDto));
                db.SaveChanges();


                return true;
            }
        }
        return false;
    }

    public bool DeleteClass(int id)
    {
        if (id != 0)
        {
            var classEntity = db.Classes.FirstOrDefault(x => x.Id == id);
            if (classEntity != null)
            {
                db.Classes.Remove(classEntity);
                db.SaveChanges();
                return true;
            }
        }
        return false;
    }
    public List<TeacherDto> GetMySubjectsByClassId(
    int classId,
    int teacherUserId)
    {
        var result =
            from assignment in db.TeachingAssignments

            where assignment.ClassId == classId
                  && assignment.TeacherUserId == teacherUserId

            join teacher in db.Users
                on assignment.TeacherUserId equals teacher.Id

            join subject in db.GeneralItems.Where(x => x.TitleType == "Subject")
                on assignment.SubjectId equals subject.Id

            select new TeacherDto
            {
                TeacherName = teacher.Name + " " + teacher.LastName,
                SubjectName = subject.Title
            };

        return result.ToList();
    }

}
