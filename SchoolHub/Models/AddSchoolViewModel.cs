using SchoolHub.Entity;

namespace SchoolHub.Models
{
    public class AddSchoolViewModel
    {
        public List<GeneralItemDto> GeneralShifts { get; set; }
        public List<GeneralItemDto> GeneralEducationPeriods { get; set; }
        public List<GeneralItemDto> GeneralEducationLevels { get; set; }
        public List<GeneralItemDto> GeneralTypes { get; set; }
        public List<GeneralItemDto> GeneralGender { get; set; }
        public List<GeneralItemDto> GeneralProrvince { get; set; }
    }
}
