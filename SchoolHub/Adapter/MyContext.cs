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
        public DbSet<EducationGradeEntity> EducationGrades { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<ScoreEntity> Scores { get; set; }
        public DbSet<SchoolEntity> Schools { get; set; }
        public DbSet<ClassEntity> Classes { get; set; }
        public DbSet<AttendanceEntity> Attendances { get; set; }
        public DbSet<DistrictEntity> Districts { get; set; }
        public DbSet<GeneralItemEntity> GeneralItems { get; set; }
        public DbSet<TeachingAssignmentEntity> TeachingAssignments { get; set; }
        public DbSet<GradeEntity> Grades { get; set; }
        public DbSet<GradeSubjectEntity> GradeSubjects { get; set; }
        public DbSet<StudentSubjectRecordEntity> StudentSubjectRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TeachingAssignmentEntity>()
                .HasKey(x => new
                {
                    x.TeacherUserId,
                    x.ClassId,
                    x.SubjectId
                });
        }
    }
}
