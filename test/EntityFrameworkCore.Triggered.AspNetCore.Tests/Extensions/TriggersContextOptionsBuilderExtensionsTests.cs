﻿using System;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.AspNetCore.Tests.Extensions
{
    public class TriggersContextOptionsBuilderExtensionsTests
    {
        public class TestModel
        {
            public Guid Id { get; set; }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public class TestDbContext : TriggeredDbContext
#pragma warning restore CS0618 // Type or member is obsolete
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

            public DbSet<TestModel> TestModels { get; set; }
        }

        [Fact]
        public void UseAspNetCoreIntegration_RegistersHttpContextServiceProviderAccessor()
        {
            IServiceProvider capturedServiceProvider = null;

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IHttpContextAccessor, Stubs.HttpContextAccessorStub>()
                .AddDbContext<TestDbContext>(options => {
                    options.UseInMemoryDatabase("UseAspNetCoreIntegration_RegistersHttpContextServiceProviderAccessor");
                    options.UseTriggers(triggerOptions => {
                        triggerOptions.UseAspNetCoreIntegration();
                        options.EnableServiceProviderCaching(false);
                    });
                })
                .AddTransient<IBeforeSaveTrigger<TestModel>>(serviceProvider => {
                    capturedServiceProvider = serviceProvider;
                    return new TriggerStub<TestModel>();
                })
                .BuildServiceProvider();


            using var serviceScope = serviceProvider.CreateScope();
            serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.RequestServices = serviceScope.ServiceProvider;
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

            dbContext.Add(new TestModel { });
            dbContext.SaveChanges();

            Assert.NotNull(capturedServiceProvider);
            Assert.Equal(serviceScope.ServiceProvider, capturedServiceProvider);
        }

        [Fact]
        public void UseAspNetCoreIntegratio_ThrowsIfIHttpContextAccessIsNotConfigured()
        {
            IServiceProvider capturedServiceProvider = null;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<TestDbContext>(options => {
                    options.UseInMemoryDatabase("UseAspNetCoreIntegratio_ThrowsIfIHttpContextAccessIsNotConfigured");
                    options.UseTriggers(triggerOptions => {
                        triggerOptions.UseAspNetCoreIntegration();
                    });
                    options.EnableServiceProviderCaching(false);
                })
                .AddTransient<IBeforeSaveTrigger<TestModel>>(serviceProvider => {
                    capturedServiceProvider = serviceProvider;
                    return new TriggerStub<TestModel>();
                })
                .BuildServiceProvider();

            using var serviceScope = serviceProvider.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

            dbContext.Add(new TestModel { });
            Assert.Throws<InvalidOperationException>(() => {
                dbContext.SaveChanges();
            });
        }
    }
}
