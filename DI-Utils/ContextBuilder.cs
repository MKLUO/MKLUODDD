using System;
namespace Microsoft.Extensions.DependencyInjection {
    using MKLUODDD.Context;
    using MKLUODDD.DAL;
    using MKLUODDD.Mapper;

    public static partial class ContextBuilderExtensions {

        public static IServiceCollection AddDefaultUtilContexts(this IServiceCollection services) {

            // Aggregation proxy
            services
                .AddScoped(typeof(IAggregationContextProxy<,>), typeof(AggregationContextProxy<,>));

            // Utility Mapper
            services
                .AddScoped<IDomainMapper, DomainMapper>();

            // Context Handle
            services
                .AddScoped<ContextHandle>()
                .AddSame<IContextHandle, ContextHandle>()
                .AddSame<IRegistableContextHandle, ContextHandle>();
            return services;
        }

        // Same instance used among multiple interfaces

        // static class ContextFactory<T> where T : class {
        //     static T? Instance { get; set; }  = null;

        //     public static T New(IServiceProvider provider) {
        //         Instance = provider.GetService<T>();
        //         return Instance;
        //     }

        //     public static T Get(IServiceProvider provider) {
        //         if (Instance == null) Instance = provider.GetService<T>();
        //         return Instance;
        //     }
        // }

        // public static IServiceCollection AddNew<IService, TService> (this IServiceCollection services) 
        //     where IService : class 
        //     where TService : class, IService {
                
        //         return services.AddTransient<IService, TService>(
        //             // x => x.GetRequiredService<TService>()
        //             x => ContextFactory<TService>.New(x)
        //         );
        //     }

        public static IServiceCollection AddSame<IService, TService> (this IServiceCollection services) 
            where IService : class 
            where TService : class, IService {
                
                return services.AddTransient<IService, TService>(
                    x => x.GetRequiredService<TService>()
                    // x => ContextFactory<TService>.Get(x)
                );
            }


        // Repo => Context builder
        #region ContextBuilder

        public interface IServiceContextBuilder<TD> 
            where TD : class, new() {

            IServiceCollection? Service { get; }

            IServiceContextBuilder<TD> WithContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TD>, IHookerContext<TD>
                where TMapper : class, IMapper<T, TD>;

            IServiceContextBuilder<TD> WithReadonlyContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TD>
                where TMapper : class, IReadonlyMapper<T, TD>;

            IServiceContextBuilder<TD> WithView<T, TMapper, TContext>()
                where T : class 
                where TContext : class, IReadContext<T>
                where TMapper : class, IViewMapper<T, TD>;
        }

        public class ServiceContextBuilder<TD> : IServiceContextBuilder<TD> 
            where TD : class, new() {

            public IServiceCollection? Service { get; set; } = null;

            public IServiceContextBuilder<TD> WithContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TD>, IHookerContext<TD>
                where TMapper : class, IMapper<T, TD> =>                 
                new ServiceContextBuilder<TD>{ Service = Service?.AddContext<T, TD, TContext, TMapper>() };

            public IServiceContextBuilder<TD> WithReadonlyContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TD>
                where TMapper : class, IReadonlyMapper<T, TD> =>                 
                new ServiceContextBuilder<TD>{ Service = Service?.AddReadonlyContext<T, TD, TContext, TMapper>() };

            public IServiceContextBuilder<TD> WithView<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>
                where TMapper : class, IViewMapper<T, TD> =>                 
                new ServiceContextBuilder<TD>{ Service = Service?.AddView<T, TD, TContext, TMapper>() };
        }

        #endregion

        public static IServiceContextBuilder<TD> AddRepository<TD>(
            this IServiceCollection services)
        where TD : class, new() => new ServiceContextBuilder<TD> { Service = services.AddScoped<Repository<TD>>() };

        public static IServiceCollection AddContext<T, TD, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TD : class, new()
            where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TD>, IHookerContext<TD>
            where TMapper : class, IMapper<T, TD> =>
                services
                    .AddScoped<TContext>()
                    .AddSame<IContext<T>, TContext>()
                    .AddSame<IPersistContext<T>, TContext>()
                    .AddSame<IAggregationContext<T, TD>, TContext>()
                    .AddSame<IHookerContext<TD>, TContext>()

                    .AddScoped<IMapper<T, TD>, TMapper>()

                    .AddSame<IWriteRepository<TD>, Repository<TD>>();
                    // .AddTransient<TContext>()
                    // .AddNew<IContext<T>, TContext>()
                    // .AddSame<IPersistContext<T>, TContext>()
                    // .AddSame<IAggregationContext<T, TD>, TContext>()
                    // .AddSame<IHookerContext<TD>, TContext>()

                    // .AddScoped<IMapper<T, TD>, TMapper>()

                    // .AddSame<IWriteRepository<TD>, Repository<TD>>();

        public static IServiceCollection AddReadonlyContext<T, TD, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TD : class, new()
            where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TD>
            where TMapper : class, IReadonlyMapper<T, TD> =>
                services
                    .AddScoped<TContext>()
                    .AddSame<IReadContext<T>, TContext>()
                    .AddSame<IPersistContext<T>, TContext>()
                    .AddSame<IAggregationContext<T, TD>, TContext>()

                    .AddScoped<IReadonlyMapper<T, TD>, TMapper>()

                    .AddSame<IBaseRepository<TD>, Repository<TD>>();

        public static IServiceCollection AddView<T, TD, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TD : class, new()
            where TContext : class, IReadContext<T>
            where TMapper : class, IViewMapper<T, TD> =>
                services
                    .AddScoped<TContext>()
                    .AddSame<IReadContext<T>, TContext>()

                    .AddScoped<IViewMapper<T, TD>, TMapper>()

                    .AddSame<IReadRepository<TD>, Repository<TD>>();
            

    }
}