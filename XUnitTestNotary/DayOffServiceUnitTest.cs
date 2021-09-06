
using EngineCoreProject.Services;
using EngineCoreProject.Services.GeneralSetting;
using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Models;
using EngineCoreProject.DTOs.MeetingDto;


using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;
using System;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;


using EngineCoreProject.IRepository.IMeetingRepository;



using Moq;

using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace XUnitTestNotary
{
    public class DayOffServiceUnitTest
    {
        public DayOffServiceUnitTest()
        {

        }

        [Fact]
        public async Task AddDayOffServiceUnitTest()
        {
            // Arrange.
            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
                        .UseInMemoryDatabase(databaseName: "Demo20")
                        .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                        .Options;

            // Create the schema in the database
            using (var context = new EngineCoreDBContext(dbOption))
            {
                context.Database.EnsureCreated();
            }
            var dbContext = new EngineCoreDBContext(dbOption);
            EngineCoreDBContext dBContext = new EngineCoreDBContext(dbOption);

            dBContext.User.Add(new User { Id = 2019, FullName = "test", UserName = "test" });
            dBContext.SaveChanges();

            Dictionary<string, string> LangValue = new Dictionary<string, string> { { "en", "val" } };

            var generalRepoMock = new Mock<IGeneralRepository>();
            generalRepoMock.Setup(m => m.getTranslationsForShortCut("res")).Returns(Task.FromResult(new Dictionary<string, string> { { "en", "val" } }));
            generalRepoMock.Setup(m => m.GenerateShortCut(Constants.DAY_OFF, Constants.DAY_OFF_REASON_SHORTCUT)).Returns("res");

            DateTime dt = DateTime.Now;
            DateTime dtAfter = DateTime.Now.AddDays(3);

            GlobalDayOffRepository repo = new GlobalDayOffRepository(dBContext, generalRepoMock.Object);
            GlobalDayOffPostDto dayOff = new GlobalDayOffPostDto { ReasonShotCutLangValue = LangValue, StartDate = dt, EndDate = dtAfter };
            GlobalDayOffPostDto intersectdayOff = new GlobalDayOffPostDto { ReasonShotCutLangValue = LangValue, StartDate = dt.AddDays(1), EndDate = dtAfter };

            // Act
            await repo.AddDayOff(dayOff, "en");
            InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddDayOff(intersectdayOff, "en"));

            // Assert
            Assert.Contains("Wrong parameter!. intersect", ex.Message);
            List<GlobalDayOffGetDto> result = repo.GetDaysOff("en").Result;
            Assert.Single(result);

            var res = result[0];
            Assert.Equal(1, res.Id);
            Assert.Equal("val", res.Reason);
            Assert.Equal(dt, res.StartDate);
            Assert.Equal(dtAfter, res.EndDate);
        }


        [Fact]
        public async Task UpdateDayOffServiceAndGetMeetingByIdUnitTest()
        {
            // Arrange.
            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
                        .UseInMemoryDatabase(databaseName: "Demo21")
                        .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                        .Options;

            // Create the schema in the database
            using (var context = new EngineCoreDBContext(dbOption))
            {
                context.Database.EnsureCreated();
            }
            var dbContext = new EngineCoreDBContext(dbOption);
            EngineCoreDBContext dBContext = new EngineCoreDBContext(dbOption);

            dBContext.User.Add(new User { Id = 2019, FullName = "test", UserName = "test" });
            dBContext.SaveChanges();

            DateTime dt = DateTime.Now;
            DateTime dtAfter = DateTime.Now.AddMinutes(10);
            Dictionary<string, string> LangValue = new Dictionary<string, string> { { "en", "val" } };

            var generalRepoMock = new Mock<IGeneralRepository>();
            generalRepoMock.Setup(m => m.getTranslationsForShortCut("res")).Returns(Task.FromResult(new Dictionary<string, string> { { "en", "val" } }));
            generalRepoMock.Setup(m => m.GenerateShortCut(Constants.DAY_OFF, Constants.DAY_OFF_REASON_SHORTCUT)).Returns("res");


            GlobalDayOffRepository repo = new GlobalDayOffRepository(dBContext, generalRepoMock.Object);
            GlobalDayOffPostDto dayOff = new GlobalDayOffPostDto { ReasonShotCutLangValue = LangValue, StartDate = dt, EndDate = dtAfter };
            GlobalDayOffPostDto secondDayOff = new GlobalDayOffPostDto { ReasonShotCutLangValue = LangValue, StartDate = dt.AddDays(10), EndDate = dtAfter.AddDays(12) };

            await repo.AddDayOff(dayOff, "en");
            await repo.AddDayOff(secondDayOff, "en");


            // Act
            GlobalDayOffPostDto updatedDayOff = new GlobalDayOffPostDto() { ReasonShotCutLangValue = LangValue, StartDate = dt.AddDays(15), EndDate = dtAfter.AddDays(17)  };
            await repo.UpdateDayOff(updatedDayOff, "ar", 1);

            // Assert
            GlobalDayOffGetDto res0 = repo.GetDayOff(1, "en").Result;
            Assert.Equal(dt.AddDays(15), res0.StartDate);
            Assert.Equal(dtAfter.AddDays(17), res0.EndDate);
            Assert.Equal("val", res0.Reason);
        }


        [Fact]
        public async Task GetDaysOffServiceUnitTest()
        {
            // Arrange.
            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
                        .UseInMemoryDatabase(databaseName: "Demo22")
                        .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                        .Options;

            // Create the schema in the database
            using (var context = new EngineCoreDBContext(dbOption))
            {
                context.Database.EnsureCreated();
            }
            var dbContext = new EngineCoreDBContext(dbOption);
            EngineCoreDBContext dBContext = new EngineCoreDBContext(dbOption);

            dBContext.User.Add(new User { Id = 2019, FullName = "test", UserName = "test" });
            dBContext.SaveChanges();

            DateTime dt = DateTime.Now;
            DateTime dtAfter = DateTime.Now.AddMinutes(10);
            Dictionary<string, string> LangValue = new Dictionary<string, string> { { "en", "val" } };

            var generalRepoMock = new Mock<IGeneralRepository>();
            generalRepoMock.Setup(m => m.getTranslationsForShortCut("res")).Returns(Task.FromResult(new Dictionary<string, string> { { "en", "val" } }));
            generalRepoMock.Setup(m => m.GenerateShortCut(Constants.DAY_OFF, Constants.DAY_OFF_REASON_SHORTCUT)).Returns("res");


            GlobalDayOffRepository repo = new GlobalDayOffRepository(dBContext, generalRepoMock.Object);
            GlobalDayOffPostDto dayOff = new GlobalDayOffPostDto { ReasonShotCutLangValue = LangValue, StartDate = dt, EndDate = dtAfter };
            GlobalDayOffPostDto secondDayOff = new GlobalDayOffPostDto { ReasonShotCutLangValue = LangValue, StartDate = dt.AddDays(10), EndDate = dtAfter.AddDays(12) };

            await repo.AddDayOff(dayOff, "en");
            await repo.AddDayOff(secondDayOff, "en");

            // Act
            List<GlobalDayOffGetDto> result = repo.GetDaysOff("en").Result;

            // Assert
            Assert.Equal(2, result.Count);

            var res0 = result[0];
            Assert.Equal(dt, res0.StartDate);
            Assert.Equal(dtAfter, res0.EndDate);
            Assert.Equal("val", res0.Reason);

            var res1 = result[1];
            Assert.Equal(dt.AddDays(10), res1.StartDate);
            Assert.Equal(dtAfter.AddDays(12), res1.EndDate);
            Assert.Equal("val", res1.Reason);
        }

    }

}

