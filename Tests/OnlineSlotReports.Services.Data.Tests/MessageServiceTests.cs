﻿namespace OnlineSlotReports.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using OnlineSlotReports.Data;
    using OnlineSlotReports.Data.Models;
    using OnlineSlotReports.Data.Repositories;
    using OnlineSlotReports.Services.Data.MessageServices;
    using OnlineSlotReports.Services.Data.Tests.Factory;
    using OnlineSlotReports.Services.Mapping;
    using OnlineSlotReports.Web.ViewModels;
    using OnlineSlotReports.Web.ViewModels.MessageViewModels;
    using Xunit;

    [Collection("Mappings collection")]
    public class MessageServiceTests
    {
        [Fact]
        public async Task AddAsyncWithCorectData()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            await service.AddAsync(
                "Ivan Ivanov",
                "content",
                "1");

            var result = await dbContextMessage.Messages.FirstOrDefaultAsync();

            Assert.Equal("Ivan Ivanov", result.Sender);
            Assert.Equal("content", result.Content);
            Assert.Equal("1", result.GamingHallId);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }

        [Fact]
        public async Task GetByIdAsyncWithCorectId()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            await service.AddAsync(
                "Ivan Ivanov",
                "content",
                "1");

            var message = await dbContextMessage.Messages.FirstOrDefaultAsync();

            var result = await service.GetByIdAsync<IndexMessageViewModel>(message.Id);

            Assert.Equal("Ivan Ivanov", result.Sender);
            Assert.Equal("content", result.Content);
            Assert.Equal("1", result.GamingHallId);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }

        [Fact]
        public async Task AllWithCorectId()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            for (int i = 1; i <= 5; i++)
            {
                await service.AddAsync(
                "Ivan Ivanov" + i,
                "content" + i,
                "222");
            }

            var results = service.All<IndexMessageViewModel>("222");

            int count = 0;
            foreach (var result in results.Reverse())
            {
                count++;
                Assert.Equal("Ivan Ivanov" + count, result.Sender);
                Assert.Equal("content" + count, result.Content);
                Assert.Equal("222", result.GamingHallId);
            }

            Assert.Equal(5, count);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }

        [Fact]
        public async Task AllWithNullId()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            for (int i = 1; i <= 5; i++)
            {
                await service.AddAsync(
                "Ivan Ivanov",
                "content",
                "1");
            }

            var results = service.All<IndexMessageViewModel>(null);

            int count = 0;
            foreach (var result in results)
            {
                count++;
            }

            Assert.Equal(0, count);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }

        [Fact]
        public async Task GetAllReadadWithOneReadMessage()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            for (int i = 1; i <= 5; i++)
            {
                await service.AddAsync(
                "Ivan Ivanov" + i,
                "content",
                "222");
            }

            var message = await dbContextMessage.Messages.Where(x => x.Sender == "Ivan Ivanov3").FirstOrDefaultAsync();
            message.Readed = true;
            await dbContextMessage.SaveChangesAsync();

            var results = service.GetAllReadById<IndexMessageViewModel>("222");

            int count = 0;
            foreach (var result in results)
            {
                count++;
                Assert.Equal("Ivan Ivanov3", result.Sender);
                Assert.Equal("content", result.Content);
                Assert.Equal("222", result.GamingHallId);
            }

            Assert.Equal(1, count);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }

        [Fact]
        public async Task DeleteAsyncWithValidId()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            await service.AddAsync(
                          "Ivan Ivanov",
                          "content",
                          "1");
            var message = await dbContextMessage.Messages.FirstOrDefaultAsync();

            await service.DeleteAsync(message.Id);

            var result = await dbContextMessage.Messages.Where(x => x.Id == message.Id).FirstOrDefaultAsync();

            Assert.True(result == null);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }

        [Fact]
        public async Task GetHallIdWithValidId()
        {
            ApplicationDbContext dbContextMessage = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var service = new MessageService(new EfDeletableEntityRepository<Message>(dbContextMessage));
            await service.AddAsync(
                   "Ivan Ivanov",
                   "content",
                   "1");
            var message = await dbContextMessage.Messages.FirstOrDefaultAsync();

            string hallId = service.GetHallId(message.Id);

            Assert.Equal("1", hallId);
            dbContextMessage.Database.EnsureDeleted();
            dbContextMessage.Dispose();
        }
    }
}
