using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class GradeSubjectEntity
    {
        [Key]
        [Required] 
        public int Id { get; set; }

        [Required]
        public int GradeId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int MajorGeneralId { get; set; }
    }
}
