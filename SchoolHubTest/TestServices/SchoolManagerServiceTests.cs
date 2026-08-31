using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Service;

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

            service = new SchoolManagerService(db, mapper);

            testService = new SchoolManagerService(testDb, mapper);
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
            Assert.True( result);
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
            Assert.False( result);
        }


        [Fact]
        public void IsUserExist_WhenNationalIdNumberIsCorrect_ReturnsTrueAndCorrectId()
        {
            // Arrange
            testDb.Users.Add(new UserEntity {GenderGeneralId=4 ,IsStudent=true , NationalIdNumber="0153054352",Name="mamad" , Id = 5, LastName="یس",Password="dddd" });
            testDb.SaveChanges();
            // Act
            var result = testService.IsUserExist("0153054352");

            // Assert
            Assert.True(result.Exist);
            Assert.Equal(5 , result.Id);
        }

        [Fact]
        public void IsUserExist_WhenNationalIdNumberIsNotCorrect_ReturnsFalseAndNotCorrectId()
        {
            // Arrange
            testDb.Users.Add(new UserEntity {GenderGeneralId=4 ,IsStudent=true , NationalIdNumber="0153054352",Name="mamad" , Id = 5, LastName="یس",Password="dddd" });
            testDb.SaveChanges();
            // Act
            var result = testService.IsUserExist("415545");

            // Assert
            Assert.False(result.Exist);
        }


        [Fact]
        public void IsStudentInClass_WhenStudentInClass_ReturnsTrue()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId =7, StudentUserId = 5 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsStudentInClass(7,5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsStudentInClass_WhenStudentDoesntInClass_ReturnsFalse()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId =7, StudentUserId = 5 });
            testDb.SaveChanges();
            // Act
            var result = service.IsStudentInClass(5,8);

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
            var result = testService.AddStudent(new StudentDto { Id = 1,ClassId=7 }, "0153054352");

            // Assert
            Assert.True(result.IsCorrect);
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