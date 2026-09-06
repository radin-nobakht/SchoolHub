using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Service;
using SchoolHub.Dto.Manager;

namespace SchoolHubTest.TestServices
{
    public class SchoolManagerServiceTests : IDisposable
    {
        private readonly MyContext db;
        private readonly MyContext testDb;

        private readonly SqliteConnection sqliteConnection;

        private readonly IMapper mapper;

        private readonly SchoolManagerService service;
        private readonly SchoolManagerService testService;
        private readonly GeneralService testGeneralService;
        private readonly GeneralService generalService;

        public SchoolManagerServiceTests()
        {
            // =========================
            // SQL Server اصلی
            // =========================

            var sqlOptions = new DbContextOptionsBuilder<MyContext>()
                .UseSqlServer(
                    "Server=.;Database=SchoolHub ;User ID=sa;Password=asdASD123;TrustServerCertificate=True;")
                .Options;

            db = new MyContext(sqlOptions);


            // =========================
            // SQLite تستی
            // =========================

            sqliteConnection = new SqliteConnection("DataSource=:memory:");
            sqliteConnection.Open();

            var sqliteOptions = new DbContextOptionsBuilder<MyContext>()
                .UseSqlite(sqliteConnection)
                .Options;

            testDb = new MyContext(sqliteOptions);

            testDb.Database.EnsureCreated();


            // =========================
            // AutoMapper
            // =========================

            using var loggerFactory = LoggerFactory.Create(builder => { });

            var mapperConfig = new MapperConfiguration(
                mc =>
                {
                    mc.AddProfile(new MappingProfile());
                },
                loggerFactory
            );

            mapper = mapperConfig.CreateMapper();


            // =========================
            // Services
            // =========================
            generalService = new GeneralService(db, mapper);
            testGeneralService = new GeneralService(testDb, mapper);

            service = new SchoolManagerService(db, mapper, generalService);

            testService = new SchoolManagerService(testDb, mapper, testGeneralService);
        }


        // =====================================================
        // SQL Server Tests
        // =====================================================

        [Fact]
        public void IsManagerOfSchool_WhenManagerOwnsSchool_ReturnsTrue()
        {
            // Act
            var result = service.IsManagerOfSchool(11, 10);

            // Assert
            Assert.True(result);
        }


        [Fact]
        public void IsManagerOfSchool_WhenManagerDoesNotOwnSchool_ReturnsFalse()
        {
            // Act
            var result = service.IsManagerOfSchool(4, 15);

            // Assert
            Assert.False(result);
        }


        // =====================================================
        // SQLite Tests
        // =====================================================


        [Fact]
        public void IsClassExist_WhenClassExist_ReturnsTrue()
        {
            // Arrange
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.SaveChanges();
            // Act
            var result = testService.IsClassExist(7);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsClassExist_WhenClassDoesntExist_ReturnsFalse()
        {
            // Arrange
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.SaveChanges();
            // Act
            var result = testService.IsClassExist(6);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void IsUserExistByNationalId_WhenNationalIdNumberIsCorrect_ReturnsTrueAndCorrectId()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, IsStudent = true, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
            testDb.SaveChanges();
            // Act
            var result = testService.IsUserExistByNationalId("0153054352");

            // Assert
            Assert.True(result.Exist);
            Assert.Equal(5, result.Id);
        }

        [Fact]
        public void IsUserExistByNationalId_WhenNationalIdNumberIsNotCorrect_ReturnsFalseAndNotCorrectId()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, IsStudent = true, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
            testDb.SaveChanges();
            // Act
            var result = testService.IsUserExistByNationalId("415545");

            // Assert
            Assert.False(result.Exist);
        }


        [Fact]
        public void IsStudentInClass_WhenStudentInClass_ReturnsTrue()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId = 7, StudentUserId = 5 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsStudentInClass(7, 5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsStudentInClass_WhenStudentDoesntInClass_ReturnsFalse()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId = 7, StudentUserId = 5 });
            testDb.SaveChanges();
            // Act
            var result = service.IsStudentInClass(5, 8);

            // Assert
            Assert.False(result);
        }



        [Fact]
        public void AddStudent_WhenAllDataIsCorrect_ReturnsTrue()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, IsStudent = true, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.SaveChanges();

            // Act
            var result = testService.AddStudent(new StudentDto { Id = 1, ClassId = 7 }, "0153054352");

            // Assert
            Assert.True(result.Success);
        }


        [Fact]
        public void AddTeacher_WhenDataIscorrect_ReturnnsTrue()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, IsStudent = true, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 21, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.GradeSubjects.AddRange(
               new GradeSubjectEntity { Id = 360, GradeId = 21, SubjectId = 1567 },
               new GradeSubjectEntity { Id = 361, GradeId = 21, SubjectId = 1568 },
               new GradeSubjectEntity { Id = 362, GradeId = 21, SubjectId = 1569 },
               new GradeSubjectEntity { Id = 363, GradeId = 21, SubjectId = 1570 },
               new GradeSubjectEntity { Id = 364, GradeId = 21, SubjectId = 1571 },
               new GradeSubjectEntity { Id = 365, GradeId = 21, SubjectId = 1572 },
               new GradeSubjectEntity { Id = 366, GradeId = 21, SubjectId = 1573 },
               new GradeSubjectEntity { Id = 367, GradeId = 21, SubjectId = 1575 },
               new GradeSubjectEntity { Id = 368, GradeId = 21, SubjectId = 1576 }
           );
            testDb.SaveChanges();

            // Act
            var result = testService.AddTeacher(new SchoolHub.Dto.Manager.TeacherDto { TeacherUserId = 0, ClassId = 7, SubjectIds = [1567, 1568, 1569] }, "0153054352");

            // Assert
            Assert.True(result.Success);
        }


        [Fact]
        public void GetGradeIdByClassId_WhenDataIsCorrect_Return4()
        {
            // Arrange
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.SaveChanges();
            // Act
            var result = testService.GetGradeIdByClassId(7);

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void GetGradeIdByClassId_WhenDataIsNotCorrect_ReturnNull()
        {
            // Arrange
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 21, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.SaveChanges();
            // Act
            var result = testService.GetGradeIdByClassId(5);

            // Assert
            Assert.Null(result);
        }



        [Fact]
        public void IsTeacherInClass_WhenTeacherInClass_ReturnsTrue()
        {
            // Arrange
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, SubjectId = 4, TeacherUserId = 8 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsTeacherInClass(5, 8);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsTeacherInClass_WhenTeacherDoesntInClass_ReturnsFalse()
        {
            // Arrange
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, SubjectId = 4, TeacherUserId = 8 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsTeacherInClass(4, 6);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void DeleteStudent_WhenStudentIsAvailable_ReturnsTrue()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 9, ClassId = 5, StudentUserId = 3 });
            testDb.SaveChanges();
            // Act
            var result = testService.DeleteStudent(9);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteStudent_WhenStudentIsNotAvailable_ReturnsFalse()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 9, ClassId = 5, StudentUserId = 3 });
            testDb.SaveChanges();
            // Act
            var result = testService.DeleteStudent(20);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DeleteTeaccher_WhenDataIsCorrect_ReturnsTrue()
        {
            // Arrange
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, TeacherUserId = 8, SubjectId = 2 });
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, TeacherUserId = 8, SubjectId = 7 });
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, TeacherUserId = 8, SubjectId = 6 });
            testDb.SaveChanges();
            // Act
            var result = testService.DeleteTeacher(5, 8);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteTeacher_WhenDataIsNotCorrect_ReturnsFalse()
        {
            // Arrange
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, TeacherUserId = 8, SubjectId = 2 });
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, TeacherUserId = 8, SubjectId = 7 });
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 5, TeacherUserId = 8, SubjectId = 6 });
            testDb.SaveChanges();
            // Act
            var result = testService.DeleteTeacher(6, 8);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void IsStudentInSchool_WhenStudentInClass_ReturnsTrue()
        {
            // Arrange
            testDb.Classes.AddRange(
             
                new ClassEntity { Id = 4, SchoolId = 7, Name = "کلاس چهارم", GradeGeneralId = 10, MajorGeneralId = 2 },
                new ClassEntity { Id = 5, SchoolId = 7, Name = "کلاس پنجم", GradeGeneralId = 11, MajorGeneralId = 2 }
            );
            testDb.Students.AddRange(
               
                new StudentEntity { StudentUserId = 10, ClassId = 4, IsDeleted = false },
                new StudentEntity { StudentUserId = 11, ClassId = 4, IsDeleted = false },
                new StudentEntity { StudentUserId = 12, ClassId = 4, IsDeleted = false },
                new StudentEntity { StudentUserId = 13, ClassId = 5, IsDeleted = false },
                new StudentEntity { StudentUserId = 14, ClassId = 5, IsDeleted = false },
                new StudentEntity { StudentUserId = 15, ClassId = 5, IsDeleted = false }
                );
            testDb.Schools.Add(new SchoolEntity {Id=7, Name = "مدرسه 1", ManagerUserId = 1, CityId = 1, DistrictId = 1, ShiftGeneralId = 1, TypeGeneralId = 1, GenderGeneralId = 1, EducationLevelGeneralId = 1, EducationPeriodGeneralId = 1 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsStudentInSchool(classId:5,studentUserId:11);

            // Assert
            Assert.True( result);
        }

        [Fact]
        public void IsStudentInSchool_WhenStudentIsntInClass_ReturnsFalse()
        {
            // Arrange
            testDb.Schools.Add(new SchoolEntity {Id=7, Name = "مدرسه 1", ManagerUserId = 1, CityId = 1, DistrictId = 1, ShiftGeneralId = 1, TypeGeneralId = 1, GenderGeneralId = 1, EducationLevelGeneralId = 1, EducationPeriodGeneralId = 1 });
            testDb.Classes.AddRange(

              new ClassEntity { Id = 4, SchoolId = 7, Name = "کلاس چهارم", GradeGeneralId = 10, MajorGeneralId = 2 },
              new ClassEntity { Id = 5, SchoolId = 7, Name = "کلاس پنجم", GradeGeneralId = 11, MajorGeneralId = 2 }
            );
            testDb.SaveChanges();
            // Act
            var result = testService.IsStudentInSchool(classId: 5, studentUserId: 11);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void IsUserExistById_WhenUserExist_ReturnsTrue()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { Id =4 , GenderGeneralId=4,IsStudent=true,LastName="d",NationalIdNumber="",Name="",Password=""});
            testDb.SaveChanges();
            // Act
            var result = testService.IsUserExistById(4);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserExistById_WhenUserIsntExist_ReturnsFalse()
        {

            // Act
            var result = testService.IsUserExistById(4);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void UpdateTeacher_WhenDataIsCorrect_ReturnsTrue()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, IsStudent = true, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 21, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.GradeSubjects.AddRange(
               new GradeSubjectEntity { Id = 360, GradeId = 21, SubjectId = 1567 },
               new GradeSubjectEntity { Id = 361, GradeId = 21, SubjectId = 1568 },
               new GradeSubjectEntity { Id = 362, GradeId = 21, SubjectId = 1569 },
               new GradeSubjectEntity { Id = 363, GradeId = 21, SubjectId = 1570 },
               new GradeSubjectEntity { Id = 364, GradeId = 21, SubjectId = 1571 },
               new GradeSubjectEntity { Id = 365, GradeId = 21, SubjectId = 1572 },
               new GradeSubjectEntity { Id = 366, GradeId = 21, SubjectId = 1573 },
               new GradeSubjectEntity { Id = 367, GradeId = 21, SubjectId = 1575 },
               new GradeSubjectEntity { Id = 368, GradeId = 21, SubjectId = 1576 }
           );
            testDb.TeachingAssignments.AddRange(
                new() { TeacherUserId = 5, ClassId = 7, SubjectId = 1567 },
                new() { TeacherUserId = 5, ClassId = 7, SubjectId = 1571 },
                new() { TeacherUserId = 5, ClassId = 7, SubjectId = 1576 }
            );
            testDb.SaveChanges();
            // Act
            var result = testService.UpdateTeacher(new SchoolHub.Dto.Manager.TeacherDto { ClassId = 7, TeacherUserId = 5, SubjectIds = [1567, 1571, 1568] });

            // Assert
            Assert.True( result.Success);
        }

        // =====================================================
        // Cleanup
        // =====================================================

        public void Dispose()
        {
            db.Dispose();
            testDb.Dispose();
            sqliteConnection.Dispose();
        }
    }
}