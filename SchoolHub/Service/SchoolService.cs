using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolHub.Adapter;
using SchoolHub.Dto;
using SchoolHub.Dto.School;
using SchoolHub.Interface;

namespace SchoolHub.Service
{
    public class SchoolService(MyContext db, IMapper mapper) : ISchoolService
    {
        public List<SchoolDto> GetSchools(int id)
        {
            var schoolIds = new List<int>();
            var schools = new List<SchoolDto>();

            foreach (var s in mapper.Map<List<SchoolDto>>(db.Schools.Where(x => x.ManagerUserId == id).ToList()))
            {
                schools.Add(s);
            }
            schoolIds = db.Classes.Where(x => x.TecherUserId == id).Select(x => x.SchoolId).ToList();
            foreach (var si in schoolIds)
            {
                schools.Add(mapper.Map<SchoolDto>(db.Schools.FirstOrDefault(x => x.Id == si)));
            }
            return schools;
        }
        public SchoolDto GetSchoolById(int id)
        {
            return mapper.Map<SchoolDto>(db.Schools.FirstOrDefault(x => x.Id == id));
        }

        public List<SchoolInfoDto> GetClasses(int schoolId)
        {
            var result = db.Classes
          .Where(c => c.SchoolId == schoolId)
          .Join(
              db.Users,
              c => c.TecherUserId,
              u => u.Id,
              (c, u) => new SchoolInfoDto
              {
                  classes = mapper.Map<ClassDto>(c),
                  Teacher = mapper.Map<UserDto>(u)
              })
          .ToList();

            return result;
        }
        public UserDto GetManagerById (int id)
        {
            return mapper.Map<UserDto>(db.Users.FirstOrDefault(x=> x.Id == id));
        }
    }
}
