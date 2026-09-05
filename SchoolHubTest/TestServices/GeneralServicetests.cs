using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolHub.Adapter;
using SchoolHub.Service;
using System;
using System.Collections.Generic;
using System.Text;
using SchoolHub.Entity;

namespace SchoolHubTest.TestServices
{
    public class GeneralServicetests
    {
        private readonly MyContext db;
        private readonly MyContext testDb;

        private readonly SqliteConnection sqliteConnection;

        private readonly IMapper mapper;

        private readonly GeneralService service;
        private readonly GeneralService testService;

        public GeneralServicetests()
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

            service = new GeneralService(db, mapper);

            testService = new GeneralService(testDb, mapper);
        }

        [Fact]
        public void IsSubjectExist_WhenSubjectsAndGradeIdAreCorrect_ReturnsTrue()
        {
            // Arrange
            testDb.GradeSubjects.AddRange(
                new GradeSubjectEntity { Id = 360, GradeId = 21, SubjectId = 1567 },
                new GradeSubjectEntity { Id = 361, GradeId = 21, SubjectId = 1568 },
                new GradeSubjectEntity { Id = 362, GradeId = 21, SubjectId = 1569 },
                new GradeSubjectEntity { Id = 363, GradeId = 21, SubjectId = 1570 },
                new GradeSubjectEntity { Id = 364, GradeId = 21, SubjectId = 1571 },
                new GradeSubjectEntity { Id = 365, GradeId = 21, SubjectId = 1572 },
                new GradeSubjectEntity { Id = 366, GradeId = 21, SubjectId = 1573 },
                new GradeSubjectEntity { Id = 367, GradeId = 21, SubjectId = 1574 },
                new GradeSubjectEntity { Id = 368, GradeId = 21, SubjectId = 1575 },
                new GradeSubjectEntity { Id = 369, GradeId = 21, SubjectId = 1576 }
            );
            testDb.SaveChanges();

            // Act
            var result = testService.IsSubjectExistForThisGrade(new List<int> { 1567, 1568, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1576 }, 21);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSubjectExist_WhenSubjectsAndGradeIdAreNotCorrect_ReturnsFalse()
        {
            // Arrange
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
            var result = testService.IsSubjectExistForThisGrade(new List<int> { 7459, 7585, 7458, 4755, 4865, 1595, 1585, 1562, 1579, 1578 }, 22);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAvailableSubjects_WhenGradeHasSubjectsAndNoSubjectIsAssignedToClass_ReturnsSubjects()
        {
            // Arrange
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
            testDb.GeneralItems.AddRange(
                new() { Id = 1567, TitleType = "Subject", ParentId = 1566, Title = "ریاضی", Active = true },
                new() { Id = 1568, TitleType = "Subject", ParentId = 1566, Title = "فارسی", Active = true },
                new() { Id = 1569, TitleType = "Subject", ParentId = 1566, Title = "علوم", Active = true },
                new() { Id = 1570, TitleType = "Subject", ParentId = 1566, Title = "قرآن", Active = true },
                new() { Id = 1571, TitleType = "Subject", ParentId = 1566, Title = "هدیه‌های آسمانی", Active = true },
                new() { Id = 1572, TitleType = "Subject", ParentId = 1566, Title = "نگارش", Active = true },
                new() { Id = 1573, TitleType = "Subject", ParentId = 1566, Title = "مطالعات اجتماعی", Active = true },
                new() { Id = 1575, TitleType = "Subject", ParentId = 1566, Title = "هنر", Active = true },
                new() { Id = 1576, TitleType = "Subject", ParentId = 1566, Title = "ورزش", Active = true }
            );
            testDb.SaveChanges();

            // Act
            var result = testService.GetAvailableSubjectsByGradeId(21, 4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(9, result.Count());

            var expectedIds = new[] { 1567, 1568, 1569, 1570, 1571, 1572, 1573, 1575, 1576 };

            Assert.Equal(
                expectedIds.OrderBy(x => x),
                result.Select(x => x.Id).OrderBy(x => x)
            );
        }
        [Fact]
        public void GetAvailableSubjectsByGradeId_WhenSomeSubjectsAreAssignedToClass_ReturnsOnlyUnassignedSubjects()
        {
            // Arrange
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
            testDb.GeneralItems.AddRange(
                new() { Id = 1567, TitleType = "Subject", ParentId = 1566, Title = "ریاضی", Active = true },
                new() { Id = 1568, TitleType = "Subject", ParentId = 1566, Title = "فارسی", Active = true },
                new() { Id = 1569, TitleType = "Subject", ParentId = 1566, Title = "علوم", Active = true },
                new() { Id = 1570, TitleType = "Subject", ParentId = 1566, Title = "قرآن", Active = true },
                new() { Id = 1571, TitleType = "Subject", ParentId = 1566, Title = "هدیه‌های آسمانی", Active = true },
                new() { Id = 1572, TitleType = "Subject", ParentId = 1566, Title = "نگارش", Active = true },
                new() { Id = 1573, TitleType = "Subject", ParentId = 1566, Title = "مطالعات اجتماعی", Active = true },
                new() { Id = 1575, TitleType = "Subject", ParentId = 1566, Title = "هنر", Active = true },
                new() { Id = 1576, TitleType = "Subject", ParentId = 1566, Title = "ورزش", Active = true }
            );
            testDb.TeachingAssignments.AddRange(
                new TeachingAssignmentEntity { ClassId = 4, SubjectId = 1567, TeacherUserId = 5},
                new TeachingAssignmentEntity { ClassId = 4, SubjectId = 1568, TeacherUserId = 5}
            );

            testDb.SaveChanges();

            // Act
            var result = testService.GetAvailableSubjectsByGradeId(21, 4);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Count());

            var expectedIds = new[] { 1569, 1570, 1571, 1572, 1573, 1575, 1576 };

            Assert.Equal(
                expectedIds.OrderBy(x => x),
                result.Select(x => x.Id).OrderBy(x => x)
            );
        }



    }


}
