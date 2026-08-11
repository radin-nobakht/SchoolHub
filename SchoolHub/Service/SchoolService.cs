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
    public List<SchoolDto> GetSchools(int id)
    {
        var schoolIds = new List<int>();
        var schools = new List<SchoolDto>();

        foreach (var s in mapper.Map<List<SchoolDto>>(db.Schools.Where(x => x.ManagerUserId == id).ToList()))
        {
            var result = (
             from district in db.GeneralItems
             join city in db.GeneralItems
              on district.ParentId equals city.Id
             join province in db.GeneralItems
              on city.ParentId equals province.Id
             where district.Id == s.DistrictId
             select new SchoolDto
             {
                 Province = province.Title,
                 City = city.Title,
                 District = district.Title
             }
             ).FirstOrDefault();

            s.Province = result.Province;
            s.City = result.City;
            s.District = result.District;

            schools.Add(s);
        }

        var ClassIds = db.TeachingAssignments.Where(x => x.TeacherUserId == id).Select(x => x.ClassId).ToList();

        foreach (var ci in ClassIds)
        {
            var clas = db.Classes.FirstOrDefault(x => x.Id == ci);
            if (clas != null)
            {
                var isDuplicate = schoolIds.FirstOrDefault(x => x == clas.SchoolId) != null ? true : false;
                if (!isDuplicate)
                    schoolIds.Add(clas.SchoolId);
            }
        }
        foreach (var si in schoolIds)
        {
            var school = db.Schools.FirstOrDefault(x => x.Id == si);
            if (school != null)
            {
                var isDuplicate = schools.FirstOrDefault(x => x.Id == school.Id) != null ? true : false;
                if (!isDuplicate)
                    schools.Add(mapper.Map<SchoolDto>(school));
            }
        }
        return schools;
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


}
