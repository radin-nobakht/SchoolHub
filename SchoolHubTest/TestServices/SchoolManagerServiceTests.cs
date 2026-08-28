using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolHub.Adapter;
using SchoolHub.Service;

namespace SchoolHubTest.TestServices
{
    public class SchoolManagerServiceTests
    {
        private readonly MyContext db;
        private readonly IMapper mapper;
        private readonly SchoolManagerService service;

        public SchoolManagerServiceTests()
        {
            // Arrange - ساخت DbContext
            var options = new DbContextOptionsBuilder<MyContext>()
            .UseSqlServer("Server=.;Database=SchoolHub;User ID=sa;Password=asdASD123;TrustServerCertificate=True;")
            .Options;

            db = new MyContext(options);

            // ساخت Logger برای AutoMapper
            using var loggerFactory = LoggerFactory.Create(builder => { });

            // ساخت Mapper واقعی پروژه
            var mapperConfig = new MapperConfiguration(
                mc =>
                {
                    mc.AddProfile(new MappingProfile());
                },
                loggerFactory
            );

            mapper = mapperConfig.CreateMapper();

            // ساخت Service
            service = new SchoolManagerService(db, mapper);
        }

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


        [Fact]
        public void Method_Scenario_ExpectedResult()
        {
          
            // Act
            var result = service.Method();

            // Assert
            Assert.Equal(expected, result);
        }

    }
}