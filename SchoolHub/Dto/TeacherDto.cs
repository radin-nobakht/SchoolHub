using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Dto
{
    public class TeacherDto
    {
       
        public int Id { get; set; }

        public int TeacherUserId { get; set; }

        public int SchoolId { get; set; }

        public bool Active { get; set; }
    }
}
