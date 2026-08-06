using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Interface;

namespace SchoolHub.Service
{
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
        public List<SchoolBarDto> GetSchoolBar(int userId)
        {
            //var querySchools = db.Schools.Where(x => x.Id == 1);
            //var queryX = db.GeneralItems.Where(x => x.Id == 1);

            //var resu = from s in db.Schools.Where(x => x.Id == 1)
            //           join x in queryX on s.Id equals x.Id
            var typeParentId = db.GeneralItems.FirstOrDefault(i => i.Title == "نوع");

            var result = (
                from type in db.GeneralItems
                where type.ParentId == typeParentId.Id

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
    }
}
