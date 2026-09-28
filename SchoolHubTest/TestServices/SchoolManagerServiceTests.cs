using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolHub.Adapter;
using SchoolHub.Dto.School;
using SchoolHub.Entity;
using SchoolHub.Service;
using SchoolHub.Dto.Manager;
using SchoolHub.Dto;

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
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
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
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
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
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.SaveChanges();

            // Act
            var result = testService.AddStudent(new StudentDto { Id = 1, ClassId = 7 }, "0153054352");

            // Assert
            Assert.True(result.Success);
        }


        [Fact]
        public void AddTeacher_WhenDataIscorrect_ReturnsTrue()

        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
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
            var result = testService.AddTeacher(new SchoolHub.Dto.Manager.TeacherDto { TeacherUserId = 5, ClassId = 7, SubjectIds = [1567, 1568, 1569] });

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
            testDb.Schools.Add(new SchoolEntity { Id = 7, Name = "مدرسه 1", ManagerUserId = 1, CityId = 1, DistrictId = 1, ShiftGeneralId = 1, TypeGeneralId = 1, GenderGeneralId = 1, EducationLevelGeneralId = 1, EducationPeriodGeneralId = 1 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsStudentInSchool(classId: 5, studentUserId: 11);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsStudentInSchool_WhenStudentIsntInClass_ReturnsFalse()
        {
            // Arrange
            testDb.Schools.Add(new SchoolEntity { Id = 7, Name = "مدرسه 1", ManagerUserId = 1, CityId = 1, DistrictId = 1, ShiftGeneralId = 1, TypeGeneralId = 1, GenderGeneralId = 1, EducationLevelGeneralId = 1, EducationPeriodGeneralId = 1 });
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
            testDb.Users.Add(new UserEntity { Id = 4, GenderGeneralId = 4, LastName = "d", NationalIdNumber = "", Name = "", Password = "" });
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
            testDb.Users.Add(new UserEntity { GenderGeneralId = 4, NationalIdNumber = "0153054352", Name = "mamad", Id = 5, LastName = "یس", Password = "dddd" });
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
            Assert.True(result.Success);
        }


        [Fact]
        public void GetClassIdByStudentAndSchoolId_WhenClassIsExist_ReturnsClassId()
        {
            // Arrange
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId = 7, StudentUserId = 5, IsDeleted = false });
            testDb.SaveChanges();

            // Act
            var result = testService.GetClassIdByStudentAndSchoolId(5, 4);

            // Assert
            Assert.Equal(7, result);
        }
        [Fact]
        public void GetClassIdByStudentAndSchoolId_WhenClassIsNotExist_Returns0()
        {
            // Arrange
            testDb.Classes.Add(new ClassEntity { Id = 7, GradeGeneralId = 4, MajorGeneralId = 8, SchoolId = 4, Name = "10.1" });
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId = 7, StudentUserId = 5, IsDeleted = false });
            testDb.SaveChanges();

            // Act
            var result = testService.GetClassIdByStudentAndSchoolId(6, 5);

            // Assert
            Assert.Equal(0, result);
        }


        [Fact]
        public void GetOverallAverage_WhenStudentHasScore_RetursnAverage()
        {
            // Arrange
            testDb.Scores.AddRange(
                new ScoreEntity { Id = 4, StudentId = 3, Score = "18", Reason = "امتحان", Status = true, Date = DateTime.Now, GeneralSubjectId = 1 },
                new ScoreEntity { Id = 5, StudentId = 3, Score = "16.5", Reason = "کلاسی", Status = true, Date = DateTime.Now, GeneralSubjectId = 1 },
                new ScoreEntity { Id = 3, StudentId = 3, Score = "19", Reason = "فعالیت", Status = true, Date = DateTime.Now, GeneralSubjectId = 2 }
            );
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId = 7, StudentUserId = 5, IsDeleted = false });
            testDb.SaveChanges();

            // Act
            var result = testService.GetOverallAverage(3);

            // Assert
            Assert.Equal(17.83, result);
        }

        [Fact]
        public void GetOverallAverage_WhenStudentHasNotScore_Retursn0()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 3, ClassId = 7, StudentUserId = 5, IsDeleted = false });
            testDb.SaveChanges();

            // Act
            var result = testService.GetOverallAverage(3);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetStudentIdByStudentAndClassId_WhenStudentExist_ReturnStudentId()
        {
            // Arrange
            testDb.Students.Add(new StudentEntity { Id = 1, ClassId = 2, StudentUserId = 3, IsDeleted = false });
            testDb.SaveChanges();
            // Act
            var result = testService.GetStudentIdByStudentAndClassId(classId: 2, studentUserId: 3);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void GetStudentIdByStudentAndClassId_WhenStudentIsntExist_Returns0()
        {

            // Act
            var result = testService.GetStudentIdByStudentAndClassId(classId: 2, studentUserId: 3);

            // Assert
            Assert.Equal(0, result);
        }


        [Fact]
        public void IsSchoolExist_WhenSchoolExist_ReturnsTrue()
        {
            // Arrange
            testDb.Schools.Add(new SchoolEntity { Id = 1, CityId = 5, DistrictId = 4, EducationLevelGeneralId = 6, EducationPeriodGeneralId = 2, GenderGeneralId = 6, ManagerUserId = 9, Name = "", ShiftGeneralId = 0, TypeGeneralId = 3 });
            testDb.SaveChanges();

            // Act
            var result = testService.IsSchoolExist(schoolId: 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsSchoolExist_WhenSchoolNotExist_ReturnsFalse()
        {


            // Act
            var result = testService.IsSchoolExist(schoolId: 1);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void GetNationalId_WhenUseIsExist_ReturnsNationalId()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 6, Id = 2, Name = "", LastName = "", NationalIdNumber = "015054352", Password = "" });
            testDb.SaveChanges();

            // Act
            var result = testService.GetNationalId(2);

            // Assert
            Assert.Equal("015054352", result);
        }

        [Fact]
        public void GetNationalId_WhenUseIsntExist_ReturnsEmpty()
        {
            // Act
            var result = testService.GetNationalId(2);

            // Assert
            Assert.Empty(result);
        }


        [Fact]
        public void GetUserByNationalId_WhenUserExist_ReturnsUser()
        {
            // Arrange
            testDb.Users.Add(new UserEntity { GenderGeneralId = 6, Id = 2, Name = "رادین", LastName = "نوبخت", NationalIdNumber = "0153054352", Password = "رادین" });
            testDb.SaveChanges();

            // Act
            var result = testService.GetUserByNationalId(nationalId: "0153054352");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("0153054352", result.NationalIdNumber);
            Assert.Equal("رادین", result.Name);
        }

        [Fact]
        public void GetUserByNationalId_WhenUserNotExist_ReturnsEmpty()
        {
            // Act
            var result = testService.GetUserByNationalId(nationalId: "015054352");

            // Assert
            Assert.True(result.Id == 0 && result.Name == null && result.LastName == null);
        }


        [Fact]
        public void UpdateSchool_WhenSchoolExist_RetrunsTrue()
        {
            // Arrange
            var school = new SchoolEntity
            {
                Name = "Test School",
                ManagerUserId = 1,
                CityId = 2,
                DistrictId = 3,
                ShiftGeneralId = 4,
                TypeGeneralId = 5,
                GenderGeneralId = 6,
                EducationLevelGeneralId = 7,
                EducationPeriodGeneralId = 8
            };

            testDb.Schools.Add(school);

            testDb.GeneralItems.AddRange(
                new GeneralItemEntity { Id = 2, Active = true, TitleType = "City", Title = "City" },
                new GeneralItemEntity { Id = 3, Active = true, TitleType = "District", ParentId = 2, Title = "District" },
                new GeneralItemEntity { Id = 4, Active = true, TitleType = "Shift", Title = "Shift" },
                new GeneralItemEntity { Id = 5, Active = true, TitleType = "Type", Title = "Type" },
                new GeneralItemEntity { Id = 6, Active = true, TitleType = "AdmissionGender", Title = "Gender" },
                new GeneralItemEntity { Id = 7, Active = true, TitleType = "EducationLevel", Title = "Level" },
                new GeneralItemEntity { Id = 8, Active = true, TitleType = "EducationPeriod", Title = "Period" }
            );
            testDb.Users.Add(new UserEntity { GenderGeneralId = 3, Id = 1, LastName = "", Name = "", NationalIdNumber = "", Password = "" });
            testDb.SaveChanges();
            // Act
            var result = testService.UpdateSchool(mapper.Map<SchoolDto>(school));

            // Assert
            Assert.True(result.IsCorrect);
        }


        [Fact]
        public void GetTeachersBySchoolId_WhenSchoolHasTeacher_ReturnsTeachers()
        {
            // Arrange
            testDb.Teachers.AddRange(
                new TeacherEntity { Id = 1, Active = true, SchoolId = 5, TeacherUserId = 6 },
                new TeacherEntity { Id = 2, Active = true, SchoolId = 5, TeacherUserId = 7 },
                new TeacherEntity { Id = 3, Active = true, SchoolId = 5, TeacherUserId = 8 },
                new TeacherEntity { Id = 4, Active = true, SchoolId = 5, TeacherUserId = 9 },
                new TeacherEntity { Id = 5, Active = false, SchoolId = 9, TeacherUserId = 10 },
                new TeacherEntity { Id = 6, Active = true, SchoolId = 10, TeacherUserId = 11 }
            );
            testDb.Users.AddRange(
                new UserEntity { Id = 6, NationalIdNumber = "", GenderGeneralId = 2, LastName = "", Name = "", Password = "" },
                new UserEntity { Id = 7, NationalIdNumber = "", GenderGeneralId = 2, LastName = "", Name = "", Password = "" },
                new UserEntity { Id = 8, NationalIdNumber = "", GenderGeneralId = 2, LastName = "", Name = "", Password = "" },
                new UserEntity { Id = 9, NationalIdNumber = "", GenderGeneralId = 2, LastName = "", Name = "", Password = "" },
                new UserEntity { Id = 10, NationalIdNumber = "", GenderGeneralId = 2, LastName = "", Name = "", Password = "" },
                new UserEntity { Id = 11, NationalIdNumber = "", GenderGeneralId = 2, LastName = "", Name = "", Password = "" }
            );
            testDb.SaveChanges();
            // Act
            var result = testService.GetTeachersBySchoolId(5);

            // Assert
            Assert.Equal(4, result.Count);
            Assert.Contains(result, x => x.UserId == 6);
            Assert.Contains(result, x => x.UserId == 7);
            Assert.Contains(result, x => x.UserId == 8);
            Assert.Contains(result, x => x.UserId == 9);
            Assert.DoesNotContain(result, x => x.UserId == 10);
            Assert.DoesNotContain(result, x => x.UserId == 11);
        }

        [Fact]
        public void GetTeachersBySchoolId_WhenSchoolHasntTeacher_ReturnsEmpty()
        {
            // Act
            var result = testService.GetTeachersBySchoolId(5);

            // Assert
            Assert.Empty(result);
        }


        [Fact]
        public void GetSchoolBySchoolId_WhenSchoolExist_ReturnsSchool()
        {
            // Arrange
            testDb.Schools.Add(new SchoolEntity { Id = 5, ManagerUserId = 1, Name = "", CityId = 2, EducationLevelGeneralId = 3, EducationPeriodGeneralId = 4, GenderGeneralId = 6, ShiftGeneralId = 2, TypeGeneralId = 2 });
            testDb.SaveChanges();


            // Act
            var result = testService.GetSchoolBySchoolId(schoolId:5);

            // Assert
            Assert.Equal(5, result.Id);
            Assert.Equal(1, result.ManagerUserId);
            Assert.Equal(2, result.CityId);
            Assert.Equal(3, result.EducationLevelGeneralId);
            Assert.Equal(4, result.EducationPeriodGeneralId);
            Assert.Equal(6, result.GenderGeneralId);
            Assert.Equal(2, result.ShiftGeneralId);
            Assert.Equal(2, result.TypeGeneralId);
        }


        [Fact]
        public void GetSchoolBySchoolId_WhenSchoolNotExist_ReturnsEmpty()
        {
            // Act
            var result = testService.GetSchoolBySchoolId(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
        }

        [Fact]
        public void GetTeacherUserIds_WhenSchoolHasTeaccher_ReturnsTeacherUserIds()
        {
            // Arrange
            testDb.Teachers.AddRange(Enumerable.Range(14, 5).Select(id => new TeacherEntity { TeacherUserId = id, SchoolId = 11, Active = true }));
            testDb.SaveChanges();

            // Act
            var result = testService.GetTeacherUserIds(schoolId:11);

            // Assert
            Assert.Equal([14, 15, 16, 17, 18], result.Order());
        }

        [Fact]
        public void GetTeacherUserIds_WhenSchoolHasntTeaccher_ReturnsEmpty()
        {
            // Act
            var result = testService.GetTeacherUserIds(schoolId:11);

            // Assert
            Assert.Empty(result);
        }


        [Fact]
        public void GetAvailableTeachersById_WhenIdsAreOk_ReturnsUsers()
        {
            // Arrange
            testDb.Users.AddRange(Enumerable.Range(1, 7).Select(id => new UserEntity { Id = id,GenderGeneralId=1,LastName="",Name="",NationalIdNumber="",Password="" }));
            testDb.TeachingAssignments.AddRange(Enumerable.Range(1, 5).Select(id => new TeachingAssignmentEntity { ClassId=8,SubjectId=5,TeacherUserId=id}));

            testDb.SaveChanges();

            // Act
            var result = testService.GetAvailableTeachersById([1, 2, 3, 4, 5, 6, 7],8);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(
                [ 6, 7],
                result.Select(x => x.Id).Order()
            );
        }

        [Fact]
        public void GetAvailableTeachersById_WhenSomeIdsDoNotExist_ReturnsEmpty()
        {
        

            // Act
            var result = testService.GetAvailableTeachersById([1, 2, 3, 6, 7],4);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteTeacherOfSchool_WhenTeacherExistButHasntClass_ReturnsTrue()
        {
            // Arrange
            testDb.Teachers.Add( new TeacherEntity { Id=5,TeacherUserId = 5 ,Active=true,SchoolId=6});
            testDb.SaveChanges();

            // Act
            var result = testService.DeleteTeacherOfSchool(teacherId:5);

            // Assert
            Assert.True(result.IsCorrect);
        }

        [Fact]
        public void DeleteTeacherOfSchool_WhenTeacherNotExist_ReturnsFalse()
        {
            // Act
            var result = testService.DeleteTeacherOfSchool(teacherId: 5);

            // Assert
            Assert.False(result.IsCorrect);
        }

        [Fact]
        public void DeleteTeacherOfSchool_WhenTeacherExistBuitHasClass_ReturnsFalse()
        {
            // Arrange
            testDb.Teachers.Add(new TeacherEntity { Id = 5, TeacherUserId = 5, Active = true, SchoolId = 6 });
            testDb.Classes.Add(new ClassEntity { GradeGeneralId = 5, Id = 6, MajorGeneralId = 4, Name = "", SchoolId = 6 });
            testDb.TeachingAssignments.Add(new TeachingAssignmentEntity { ClassId = 6, SubjectId = 54, TeacherUserId = 5 });
            testDb.SaveChanges();

            // Act
            var result = testService.DeleteTeacherOfSchool(teacherId: 5);

            // Assert
            Assert.False(result.IsCorrect);
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