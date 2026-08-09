using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Entity;
using SchoolHub.Dto.School;
using Microsoft.AspNetCore.Mvc;
using SchoolHub.Interface;
using SchoolHub.Models;

namespace SchoolHub.Service;

public class SchoolService(MyContext db, IMapper mapper) : ISchoolService
{
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
        var result = db.Classes.Where(x => x.SchoolId == schoolId).ToList();

        return mapper.Map<List<ClassDto>>(result);
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
        if (classDto.Name != null && classDto.GradeId != 0)
        {

            var isDuplicate = db.Classes.FirstOrDefault(x => x.GradeId == classDto.GradeId && x.Name == classDto.Name && x.SchoolId == classDto.SchoolId) == null ? false : true;
            if (!isDuplicate)
            {
                db.Add(mapper.Map<ClassEntity>(classDto));
                db.SaveChanges();
                db.TeachingAssignments.Add(new TeachingAssignmentEntity
                {
                    ClassId = classDto.Id
                });

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
