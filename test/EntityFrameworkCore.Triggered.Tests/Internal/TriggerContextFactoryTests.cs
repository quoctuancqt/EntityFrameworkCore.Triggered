﻿using System;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerContextFactoryTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase(nameof(TriggerContextFactoryTests));
                optionsBuilder.EnableServiceProviderCaching(false);
            }
        }

        [Fact]
        public void Activate_ReturnsInstance()
        {
            using var dbContext = new TestDbContext();
            var entityEntry = dbContext.Entry(new TestModel { });

            var result = TriggerContextFactory<object>.Activate(entityEntry, entityEntry.OriginalValues, ChangeType.Added);

            Assert.NotNull(result);
        }
    }
}
