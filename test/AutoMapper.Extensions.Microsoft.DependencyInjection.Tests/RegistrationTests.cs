﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AutoMapper.Extensions.Microsoft.DependencyInjection.Tests
{
    [Collection(nameof(RegistrationTests))]
    public class RegistrationTests
    {
        [Fact]
        public void Should_not_register_static_instance_when_configured()
        {
            ServiceCollectionExtensions.UseStaticRegistration = false;

            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IProfile3Dependency, Profile3Dependency>();
            services.AddAutoMapper();

            var serviceProvider = services.BuildServiceProvider();

            var config = serviceProvider.GetService<IConfigurationProvider>();
            config.ShouldNotBeNull();

            try
            {
                config.ShouldNotBeSameAs(Mapper.Configuration);
            }
            catch (InvalidOperationException)
            {
                // Success if the mapper has not been initialized anyway
            }
        }
    }
}