
using EngineCoreProject.Services;
using EngineCoreProject.Services.Meetings;
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
using EngineCoreProject.Services.UserService;

namespace XUnitTestNotary
{
    public class MeetingServiceUnitTest
    {

        public MeetingServiceUnitTest()
        {

        }


        /*
         * 
         * 
        [Fact]
        public async Task AddMeetingServiceUnitTest()
        {

            // Arrange.

         //   var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
           //             .UseInMemoryDatabase(databaseName: "Demo")
             //           .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               //         .Options;
            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
            .UseInMemoryDatabase(databaseName: "Demo112")
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

            MeetingRepository repo = new MeetingRepository(dBContext);
            MeetingPostDto meet = new MeetingPostDto { Description = "desc", EndDate = dtAfter, Password = "1234", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz", Topic = "112" };
            MeetingPostDto meet1 = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz1", Topic = "1121" };
            MeetingPostDto meetExisted = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz1", Topic = "1121" };
            MeetingPostDto meetWrongStatus = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 10, TimeZone = "tz1", Topic = "1121" };
            MeetingPostDto meetWrongDate = new MeetingPostDto { Description = "desc1", EndDate = dt, Password = "12341", PasswordReq = true, StartDate = dtAfter, Status = 0, TimeZone = "tz1", Topic = "1121" };


            // Act
            await repo.AddMeeting(meet, 2019, "ar");
            await repo.AddMeeting(meet1, 2019, "ar");
            InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddMeeting(meetExisted, 2019, "en"));
            Assert.Contains("parameter value existed before!. 14521", ex.Message);
            InvalidOperationException exWrongStatus = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddMeeting(meetWrongStatus, 2019, "en"));
            Assert.Contains("Wrong parameter!. 10", exWrongStatus.Message);
            InvalidOperationException exWrongDate = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.AddMeeting(meetWrongDate, 2019, "en"));
            Assert.Contains("Wrong parameter!", exWrongDate.Message);

            // Assert
            List<MeetingGetDto> result = repo.GetMeetings(2019, "ar").Result;
            Assert.Equal(2, result.Count);
            var res0 = result[0];
            Assert.Equal("1452", res0.MeetingId);
            Assert.Equal("1234", res0.Password);
            Assert.Equal(true, res0.PasswordReq);
            Assert.Equal(dt, res0.StartDate);
            Assert.Equal(dtAfter, res0.EndDate);
            Assert.Equal("desc", res0.Description);
            Assert.Equal(0, res0.Status);
            Assert.Equal("tz", res0.TimeZone);
            Assert.Equal("112", res0.Topic);

            res0 = result[1];
            Assert.Equal("14521", res0.MeetingId);
            Assert.Equal("12341", res0.Password);
            Assert.Equal(true, res0.PasswordReq);
            Assert.Equal(dt, res0.StartDate);
            Assert.Equal(dtAfter, res0.EndDate);
            Assert.Equal("desc1", res0.Description);
            Assert.Equal(0, res0.Status);
            Assert.Equal("tz1", res0.TimeZone);
            Assert.Equal("1121", res0.Topic);

            // free resources.
            dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task UpdateMeetingServiceAndGetMeetingByIdUnitTest()
        {
            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
               .UseInMemoryDatabase(databaseName: "Demo1")
               .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;

            // Create the schema in the database
            using (var context = new EngineCoreDBContext(dbOption))
            {
                context.Database.EnsureCreated();
            }
            var dbContext = new EngineCoreDBContext(dbOption);
            EngineCoreDBContext dBContext = new EngineCoreDBContext(dbOption);
            dBContext.User.Add(new User { Id = 2019, FullName = "test", UserName = "test"  });
            dBContext.SaveChanges();

            DateTime dt = DateTime.Now;
            DateTime dtAfter = DateTime.Now.AddMinutes(10);

            MeetingRepository repo = new MeetingRepository(dBContext);
            MeetingPostDto meet = new MeetingPostDto { Description = "desc", EndDate = dtAfter, Password = "1234", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz", Topic = "112" };
            MeetingPostDto meet1 = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz1", Topic = "1121" };
            MeetingPostDto meetExisted = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz1", Topic = "1121" };
            MeetingPostDto meetWrongStatus = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 2, TimeZone = "tz1", Topic = "1121" };
            MeetingPostDto meetWrongDate = new MeetingPostDto { Description = "desc1", EndDate = dt, Password = "12341", PasswordReq = true, StartDate = dtAfter, Status = 0, TimeZone = "tz1", Topic = "1121" };

            await repo.AddMeeting(meet, 2019, "ar");
            await repo.AddMeeting(meet1, 2019, "ar");
            DateTime sdt = DateTime.Now.AddDays(5);
            DateTime fdt = DateTime.Now.AddDays(6);

            // Act
            MeetingPostDto updatedMeet = new MeetingPostDto() { Description = "updatedDesc", StartDate = sdt, EndDate = fdt, Password = "updatedPass", PasswordReq = true, Status = -1, TimeZone = "updatedTZ", Topic = "updatedTopic" };
            await repo.UpdateMeeting(1, updatedMeet, 2019, "ar");

            //   InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.UpdateMeeting(2, meetExisted, 2019, "en"));
            //  Assert.Contains("parameter value existed before!. 14521", ex.Message);

            InvalidOperationException exWrongStatus = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.UpdateMeeting(2, meetWrongStatus, 2019, "en"));
            Assert.Contains("Wrong parameter!. 2", exWrongStatus.Message);
            InvalidOperationException exWrongDate = await Assert.ThrowsAsync<InvalidOperationException>(() => repo.UpdateMeeting(2, meetWrongDate, 2019, "en"));
            Assert.Contains("Wrong parameter!", exWrongDate.Message);

            // Assert
            MeetingGetDto res0 = repo.GetMeetingById(1, "ar").Result;
            Assert.Equal("updatedMeet", res0.MeetingId);
            Assert.Equal("updatedPass", res0.Password);
            Assert.Equal(true, res0.PasswordReq);
            Assert.Equal(sdt, res0.StartDate);
            Assert.Equal(fdt, res0.EndDate);
            Assert.Equal("updatedDesc", res0.Description);
            Assert.Equal(-1, res0.Status);
            Assert.Equal("updatedTZ", res0.TimeZone);
            Assert.Equal("updatedTopic", res0.Topic);

            // free resources.
            dbContext.Database.EnsureDeleted();
        }


        [Fact]
        public void GetMeetingServiceUnitTest()
        {
            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
                 .UseInMemoryDatabase(databaseName: "Demo2")
                 .Options;

            DateTime dt = DateTime.Now;
            DateTime dtAfter = DateTime.Now.AddMinutes(10);

            EngineCoreDBContext dBContext = new EngineCoreDBContext(dbOption);
            dBContext.Meeting.Add(new Meeting { Id = 2, MeetingId = "123", UserId = 2019, Description = "desc", EndDate = dtAfter, Password = "pass", StartDate = dt, Status = 0, Topic = "topic", TimeZone = "tz", PasswordReq = true });
            dBContext.Meeting.Add(new Meeting { Id = 1, MeetingId = "12345", UserId = 2019, Description = "desc1", EndDate = dtAfter, Password = "pass1", StartDate = dt, Status = 0, Topic = "topic1", TimeZone = "tz1", PasswordReq = false });

            dBContext.User.Add(new User { Id = 2019, FullName = "test", UserName = "test" });
            dBContext.SaveChanges();

            MeetingRepository repo = new MeetingRepository(dBContext);

            // Act
            List<MeetingGetDto> result = repo.GetMeetings(2019, "ar").Result;

            // Assert
            Assert.Equal(2, result.Count);
            var res0 = result[0];
            Assert.Equal("123", res0.MeetingId);
            Assert.Equal("pass", res0.Password);
            Assert.Equal(true, res0.PasswordReq);
            Assert.Equal(dt, res0.StartDate);
            Assert.Equal(dtAfter, res0.EndDate);
            Assert.Equal("desc", res0.Description);
            Assert.Equal(0, res0.Status);
            Assert.Equal("tz", res0.TimeZone);

            var res1 = result[1];
            Assert.Equal("12345", res1.MeetingId);
            Assert.Equal("pass1", res1.Password);
            Assert.Equal(false, res1.PasswordReq);
            Assert.Equal(dt, res1.StartDate);
            Assert.Equal(dtAfter, res1.EndDate);
            Assert.Equal("desc1", res1.Description);
            Assert.Equal(0, res1.Status);
            Assert.Equal("tz1", res1.TimeZone);

            // free resources.
            dBContext.Database.EnsureDeleted();
        }


        [Fact]
        public async Task GetMeetingByIdandByPassWordServiceUnitTest()
        {

            // Arrange.

            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
                        .UseInMemoryDatabase(databaseName: "Demo3")
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

            MeetingRepository repo = new MeetingRepository(dBContext);
            MeetingPostDto meet = new MeetingPostDto { Description = "desc", EndDate = dtAfter, Password = "1234", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz", Topic = "112" };
            MeetingPostDto meet1 = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 1, TimeZone = "tz1", Topic = "1121" };

            await repo.AddMeeting(meet, 2019, "ar");
            await repo.AddMeeting(meet1, 2019, "ar");

            // Act

            MeetingGetDto result = await repo.GetMeetingByMeetingId("1452", "ar");
            MeetingGetDto resultPass = await repo.GetMeetingByMeetingIdAndPassword("14521", "12341", "ar");

            // Assert
            Assert.Equal(0, (int)await repo.GetMeetingStatus("1452", "1234", "ar"));
            Assert.Equal(1, (int)await repo.GetMeetingStatus("14521", "12341", "ar"));
            Assert.Equal("1452", result.MeetingId);
            Assert.Equal("1234", result.Password);
            Assert.Equal(true, result.PasswordReq);
            Assert.Equal(dt, result.StartDate);
            Assert.Equal(dtAfter, result.EndDate);
            Assert.Equal("desc", result.Description);
            Assert.Equal(0, result.Status);
            Assert.Equal("tz", result.TimeZone);
            Assert.Equal("112", result.Topic);

            Assert.Equal("14521", resultPass.MeetingId);
            Assert.Equal("12341", resultPass.Password);
            Assert.Equal(true, resultPass.PasswordReq);
            Assert.Equal(dt, resultPass.StartDate);
            Assert.Equal(dtAfter, resultPass.EndDate);
            Assert.Equal("desc1", resultPass.Description);
            Assert.Equal(1, resultPass.Status);
            Assert.Equal("tz1", resultPass.TimeZone);
            Assert.Equal("1121", resultPass.Topic);

            // free resources.
            dbContext.Database.EnsureDeleted();
        }




        [Fact]
        public async Task SetStatusMeetingServiceUnitTest()
        {

            // Arrange.

            var dbOption = new DbContextOptionsBuilder<EngineCoreDBContext>()
                        .UseInMemoryDatabase(databaseName: "Demo4")
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

            MeetingRepository repo = new MeetingRepository(dBContext);
            MeetingPostDto meet = new MeetingPostDto { Description = "desc", EndDate = dtAfter, Password = "1234", PasswordReq = true, StartDate = dt, Status = 0, TimeZone = "tz", Topic = "112" };
            MeetingPostDto meet1 = new MeetingPostDto { Description = "desc1", EndDate = dtAfter, Password = "12341", PasswordReq = true, StartDate = dt, Status = 1, TimeZone = "tz1", Topic = "1121" };

            await repo.AddMeeting(meet, 2019, "ar");
            await repo.AddMeeting(meet1, 2019, "ar");

            // Act
            await repo.SetMeetingStatus("1452", Constants.MEETING_STATUS.FINISHED , "ar");
            await repo.SetMeetingStatus("14521", Constants.MEETING_STATUS.FINISHED, "ar");

            Assert.True(await repo.MeetingHasPassword("1452"));
            Assert.True(await repo.MeetingHasPassword("14521"));

            // Assert
            MeetingGetDto result = await repo.GetMeetingByMeetingId("1452", "ar");
            MeetingGetDto resultPass = await repo.GetMeetingByMeetingIdAndPassword("14521", "12341", "ar");

            Assert.Equal("1452", result.MeetingId);
            Assert.Equal("1234", result.Password);
            Assert.Equal(true, result.PasswordReq);
            Assert.Equal(dt, result.StartDate);
            Assert.Equal(dtAfter, result.EndDate);
            Assert.Equal("desc", result.Description);
            Assert.Equal(-1, result.Status);
            Assert.Equal("tz", result.TimeZone);
            Assert.Equal("112", result.Topic);

            Assert.Equal("14521", resultPass.MeetingId);
            Assert.Equal("12341", resultPass.Password);
            Assert.Equal(true, resultPass.PasswordReq);
            Assert.Equal(dt, resultPass.StartDate);
            Assert.Equal(dtAfter, resultPass.EndDate);
            Assert.Equal("desc1", resultPass.Description);
            Assert.Equal(-1, resultPass.Status);
            Assert.Equal("tz1", resultPass.TimeZone);
            Assert.Equal("1121", resultPass.Topic);

            // free resources.
            dbContext.Database.EnsureDeleted();
        }

        */
    }

        
}

