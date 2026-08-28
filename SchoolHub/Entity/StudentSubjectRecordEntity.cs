using Microsoft.EntityFrameworkCore;

namespace SchoolHub.Entity
{
    [Keyless]
    public class StudentSubjectRecordEntity
    {
        public int StudentUserId { get; set; }

        public int SubjectGeneralId { get; set; }

        public string Type { get; set; }

        public int Count { get; set; }

        public string Reason { get; set; }

        public DateOnly Date { get; set; }

    }
}
