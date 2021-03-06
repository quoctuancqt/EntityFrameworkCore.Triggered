﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Extensions
{
    public class DbContextExtensionTests
    {
        class TestModel { public int Id { get; set; } public string Name { get; set; } }

        class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseInMemoryDatabase("test").UseTriggers();
        }

        [Fact]
        public void CreateTriggerSession_ValidDbContext_CreatesNewSession()
        {
            var context = new TestDbContext();
            var triggerSession = DbContextExtensions.CreateTriggerSession(context);

            Assert.NotNull(triggerSession);
            Assert.NotNull(context.GetService<ITriggerService>()?.Current);
        }
    }
}
