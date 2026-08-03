using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Dto.School
{
    public class SchoolDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManagerUserId { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int ShiftGeneralId { get; set; }
        public int TypeGeneralId { get; set; }
        public int GenderGeneralId { get; set; }
        public int EducationLevelGeneralId { get; set; }
        public string Province {  get; set; }
        public string City { get; set; }
        public int District { get; set; }
    }
}
