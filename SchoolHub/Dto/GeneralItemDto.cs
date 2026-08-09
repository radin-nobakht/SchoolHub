using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class GeneralItemDto
    {
       
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string TitleType { get; set; }
        public string Title { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; }
    }
}
