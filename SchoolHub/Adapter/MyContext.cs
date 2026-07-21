using Microsoft.EntityFrameworkCore;
using SchoolHub.Entity;

namespace SchoolHub.Adapter
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
        : base(options)
        {
            Database.SetCommandTimeout(30);
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<ScoreEntity> Scores { get; set; }
        public DbSet<SchoolEntity> Schools { get; set; }
        public DbSet<ClassEntity> Classes { get; set; }
        public DbSet<AttendanceEntity> Attendances { get; set; }
    }
}
