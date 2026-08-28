using AutoMapper;
using Microsoft.Identity.Client;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Interface;

namespace SchoolHub.Service
{
    public class GeneralService(MyContext db, IMapper mapper) : IGeneralService
    {
        public List<GeneralItemDto> GetCitiesByProvinceId(int provinceId)
        {
            return mapper.Map<List<GeneralItemDto>>(
                db.GeneralItems.Where(x => x.ParentId == provinceId).ToList()
            );
        }

        public List<GeneralItemDto> GetDistrictByCityId(int cityId)
        {
            var districts = db.GeneralItems.Where(x => x.ParentId == cityId).ToList();

            if (!districts.Any())
                return null;

            return mapper.Map<List<GeneralItemDto>>(districts);
        }

        public Dictionary<string, List<GeneralItemDto>> GetGeneralItems(string[] titles)
        {
            var generalCategory = db
                .GeneralItems.Where(x => titles.Contains(x.Title) && x.ParentId == null)
                .ToList();
            var categoryIds = generalCategory.Select(x => x.Id).ToList();
            var generalItems = db
                .GeneralItems.Where(x =>
                    x.ParentId.HasValue && categoryIds.Contains(x.ParentId.Value)
                )
                .ToList();
            return generalCategory.ToDictionary(
                category => category.Title,
                category =>
                    generalItems
                        .Where(item => item.ParentId == category.Id)
                        .Select(item => new GeneralItemDto { Id = item.Id, Title = item.Title })
                        .ToList()
            );
        }

        public async Task<FullSchoolDataDto> GetFullDataOfSchoolById(int schoolId)
        {
            #region schoolJoin
            var result = (
                from school in db.Schools
                where school.Id == schoolId

                join manager in db.Users on school.ManagerUserId equals manager.Id

                join city in db.GeneralItems on school.CityId equals city.Id

                join province in db.GeneralItems on city.ParentId equals province.Id

                join district in db.GeneralItems on school.DistrictId equals district.Id

                join type in db.GeneralItems on school.TypeGeneralId equals type.Id

                join gender in db.GeneralItems on school.GenderGeneralId equals gender.Id

                join Shift in db.GeneralItems on school.ShiftGeneralId equals Shift.Id

                join educationLevel in db.GeneralItems
                    on school.EducationLevelGeneralId equals educationLevel.Id

                join educationPeriod in db.GeneralItems
                    on school.EducationPeriodGeneralId equals educationPeriod.Id

                select new FullSchoolDataDto
                {
                    Id = schoolId,
                    Name = school.Name,
                    MangerFullName = (manager.Name + " " + manager.LastName),
                    Province = province.Title,
                    City = city.Title,
                    District = district.Title,
                    Type = type.Title,
                    Gender = gender.Title,
                    Shift = Shift.Title,
                    EducationLevel = educationLevel.Title,
                    EducationPeriod = educationPeriod.Title,
                    MangerUserId = school.ManagerUserId,
                }
            ).FirstOrDefault();
            #endregion

            return result;
        }

        public List<GeneralItemDto> GetGeneralGrades(int schoolId)
        {
            var school = db
                .Schools.Where(x => x.Id == schoolId)
                .Select(x => new
                {
                    EducationLevelGeneralId = x.EducationLevelGeneralId,
                    EducationPeriodGeneralId = x.EducationPeriodGeneralId,
                })
                .FirstOrDefault();

            var gradeIds = db
                .EducationGrades.Where(x =>
                    x.GeneralEducationPeriodId == school.EducationPeriodGeneralId
                    && x.GeneralEducationLevelId == school.EducationLevelGeneralId
                )
                .Select(x => x.GeneralGradeId)
                .ToList();
            return mapper.Map<List<GeneralItemDto>>(
                db.GeneralItems.Where(x => gradeIds.Contains(x.Id))
            );
        }

        public List<GeneralItemDto> GetGenerals(string type, int? parentId = null) => mapper.Map<List<GeneralItemDto>>(db.GeneralItems.Where(x => x.TitleType == type && (parentId != null ? x.ParentId == parentId : true)).ToList());
        

        public GeneralItemDto GetCurrentItem(int classId, string type)
        {
            var clas = db.Classes.FirstOrDefault(x => x.Id == classId) ?? new ClassEntity();
            var generalItem = db.GeneralItems.FirstOrDefault(x => (x.Id == clas.GradeGeneralId || x.Id == clas.MajorGeneralId) && x.TitleType == type);

            return mapper.Map<GeneralItemDto>(generalItem);
        }
    }
}
