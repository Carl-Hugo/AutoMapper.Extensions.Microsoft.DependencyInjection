﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp
{
    using System.Reflection;
    using AutoMapper;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<ISomeService>(sp => new FooService(5));
            services.AddTransient<IProfile3Dependency, Profile3Dependency>();
            services.AddAutoMapper(typeof(Source));
            var provider = services.BuildServiceProvider();
            provider.GetService<IMapper>();

            foreach (var service in services)
            {
                Console.WriteLine(service.ServiceType + " - " + service.ImplementationType);
            }
            Console.ReadKey();
        }
    }

    public class Source
    {
    }

    public class Dest
    {
    }

    public class Source2
    {
    }

    public class Dest2
    {
        public int ResolvedValue { get; set; }
    }

    public class Source3
    {
    }

    public class Dest3
    {
    }

    public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Source, Dest>();
        }
    }

    public class Profile2 : Profile
    {
        public Profile2()
        {
            CreateMap<Source2, Dest2>()
                .ForMember(d => d.ResolvedValue, opt => opt.ResolveUsing<DependencyResolver>());
        }
    }

    public class Profile3 : Profile
    {
        public Profile3(IProfile3Dependency profile3Dependency)
        {
            if (profile3Dependency == null) { throw new ArgumentNullException(nameof(profile3Dependency)); }
            CreateMap<Source3, Dest3>();
        }
    }

    public interface IProfile3Dependency { }
    public class Profile3Dependency : IProfile3Dependency { }

    public class DependencyResolver : IValueResolver<object, object, int>
    {
        private readonly ISomeService _service;

        public DependencyResolver(ISomeService service)
        {
            _service = service;
        }

        public int Resolve(object source, object destination, int destMember, ResolutionContext context)
        {
            return _service.Modify(destMember);
        }
    }

    public interface ISomeService
    {
        int Modify(int value);
    }

    public class FooService : ISomeService
    {
        private readonly int _value;

        public FooService(int value)
        {
            _value = value;
        }

        public int Modify(int value) => value + _value;
    }
}
