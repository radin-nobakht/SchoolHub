using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Interface;

namespace SchoolHub.Service;

public class MenuService(MyContext db) : IMenuService

{

    //public List<SchoolEntity> Filter(string? name, string? desc)
    //{

    //    var query = db.Schools.Where(x=>x.CityId==1);
    //    if (!string.IsNullOrEmpty(name))
    //        query = query.Where(x => x.Name == name);
    //    if (!string.IsNullOrEmpty(desc))
    //        query = query.Where(x => x.Name == desc);
    //    var strQuery = query.ToQueryString();
    //    return query.ToList();
    //}
    public List<SchoolBarDto> GetSchoolThatManagerBar(int userId)
    {
        //var querySchools = db.Schools.Where(x => x.Id == 1);
        //var queryX = db.GeneralItems.Where(x => x.Id == 1);

        //var resu = from s in db.Schools.Where(x => x.Id == 1)
        //           join x in queryX on s.Id equals x.Id

        var result = (
            from type in db.GeneralItems
            where type.TitleType == "Type"

            join school in db.Schools.Where(x => x.ManagerUserId == userId)
                on type.Id equals school.TypeGeneralId

            select new
            {
                Type = type.Title,
                School = new School
                {
                    Id = school.Id,
                    Name = school.Name
                }
            })
            .GroupBy(x => x.Type)
            .Select(g => new SchoolBarDto
            {
                SchoolType = g.Key,
                Schools = g.Select(x => x.School).ToList()
            })
            .ToList();

        return result;

    }
    public List<SchoolBarDto> GetSchoolThatTeacherBar(int userId)
    {
        var result = (
  from assignment in db.TeachingAssignments
  where assignment.TeacherUserId == userId

  join schoolClass in db.Classes
      on assignment.ClassId equals schoolClass.Id

  join school in db.Schools
      on schoolClass.SchoolId equals school.Id

  join type in db.GeneralItems
      on school.TypeGeneralId equals type.Id

  where type.TitleType == "Type"

  select new
  {
      Type = type.Title,
      SchoolId = school.Id,
      SchoolName = school.Name
  })
  .Distinct()
  .AsEnumerable()
  .GroupBy(x => x.Type)
  .Select(g => new SchoolBarDto
  {
      SchoolType = g.Key,
      Schools = g.Select(x => new School
      {
          Id = x.SchoolId,
          Name = x.SchoolName
      }).ToList()
  })
  .ToList();

        return result;
    }

    public List<SchoolBarDto> GetSchoolThatStudentrBar(int userId)
    {
        var result = (
            from student in db.Students
            where student.StudentUserId == userId

            join schoolClass in db.Classes
                on student.ClassId equals schoolClass.Id

            join school in db.Schools
                on schoolClass.SchoolId equals school.Id

            join type in db.GeneralItems
                on school.TypeGeneralId equals type.Id

            where type.TitleType == "Type"

            select new
            {
                Type = type.Title,
                SchoolId = school.Id,
                SchoolName = school.Name
            })
            .AsNoTracking()
            .Distinct()
            .AsEnumerable()
            .GroupBy(x => x.Type)
            .Select(g => new SchoolBarDto
            {
                SchoolType = g.Key,

                Schools = g.Select(x => new School
                {
                    Id = x.SchoolId,
                    Name = x.SchoolName
                }).ToList()
            })
            .ToList();

        return result;
    }
}

