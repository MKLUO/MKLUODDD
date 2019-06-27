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

        public interface IServiceContextBuilder<TORM> 
            where TORM : class, new() {

            IServiceCollection Service { get; }

            IServiceContextBuilder<TORM> WithContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>, IHookerContext<TORM>
                where TMapper : class, IMapper<T, TORM>;

            IServiceContextBuilder<TORM> WithLookUpContext<T, TChild, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>, IHookerContext<TORM>,ILookUpContext<TChild, T>
                where TMapper : class, IMapper<T, TORM>;

            IServiceContextBuilder<TORM> WithReadonlyContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>
                where TMapper : class, IReadonlyMapper<T, TORM>;

            IServiceContextBuilder<TORM> WithView<T, TMapper, TContext>()
                where T : class 
                where TContext : class, IReadContext<T>
                where TMapper : class, IViewMapper<T, TORM>;

            IServiceCollection End();
        }

        public class ServiceContextBuilder<TORM> : IServiceContextBuilder<TORM> 
            where TORM : class, new() {

            public IServiceCollection Service { get; set; }

            public ServiceContextBuilder(IServiceCollection service) =>
                Service = service;

            public IServiceContextBuilder<TORM> WithContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>, IHookerContext<TORM>
                where TMapper : class, IMapper<T, TORM> =>                 
                new ServiceContextBuilder<TORM>(service: Service.AddContext<T, TORM, TContext, TMapper>());

            public IServiceContextBuilder<TORM> WithLookUpContext<T, TChild, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>, IHookerContext<TORM>,ILookUpContext<TChild, T>
                where TMapper : class, IMapper<T, TORM> =>                 
                new ServiceContextBuilder<TORM>(service: Service.AddLookUpContext<T, TORM, TChild, TContext, TMapper>());

            public IServiceContextBuilder<TORM> WithReadonlyContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>
                where TMapper : class, IReadonlyMapper<T, TORM> =>                 
                new ServiceContextBuilder<TORM>(service: Service.AddReadonlyContext<T, TORM, TContext, TMapper>());

            public IServiceContextBuilder<TORM> WithView<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>
                where TMapper : class, IViewMapper<T, TORM> =>                 
                new ServiceContextBuilder<TORM>(service: Service.AddView<T, TORM, TContext, TMapper>());

            public IServiceCollection End() => Service;
        }

        #endregion

        public static IServiceContextBuilder<TORM> AddRepository<TORM>(
            this IServiceCollection services)
        where TORM : class, new() => new ServiceContextBuilder<TORM>(service: services.AddScoped<Repository<TORM>>());

        static IServiceCollection AddBaseContext<T, TORM, TContext>(
            this IServiceCollection services)
            where T : class
            where TORM : class, new()
            where TContext : class, IPersistContext<T>, IAggregationContext<T, TORM> =>
                services
                    .AddScoped<TContext>()
                    .AddSame<IPersistContext<T>, TContext>()
                    .AddSame<IAggregationContext<T, TORM>, TContext>();

        public static IServiceCollection AddContext<T, TORM, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TORM : class, new()
            where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>, IHookerContext<TORM>
            where TMapper : class, IMapper<T, TORM> =>
                services
                    .AddBaseContext<T, TORM, TContext>()
                    .AddSame<IContext<T>, TContext>()
                    .AddSame<IHookerContext<TORM>, TContext>()
                    .AddScoped<IMapper<T, TORM>, TMapper>()
                    .AddSame<IWriteRepository<TORM>, Repository<TORM>>();
        
        public static IServiceCollection AddLookUpContext<T, TORM, TChild, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TORM : class, new()
            where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>, IHookerContext<TORM>,ILookUpContext<TChild, T>
            where TMapper : class, IMapper<T, TORM> =>
                services
                    .AddContext<T, TORM, TContext, TMapper>()
                    .AddSame<ILookUpContext<TChild, T>, TContext>();

        public static IServiceCollection AddReadonlyContext<T, TORM, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TORM : class, new()
            where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TORM>
            where TMapper : class, IReadonlyMapper<T, TORM> =>
                services
                    .AddBaseContext<T, TORM, TContext>()
                    .AddSame<IReadContext<T>, TContext>()
                    .AddScoped<IReadonlyMapper<T, TORM>, TMapper>()
                    .AddSame<IBaseRepository<TORM>, Repository<TORM>>();

        public static IServiceCollection AddView<T, TORM, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TORM : class, new()
            where TContext : class, IReadContext<T>
            where TMapper : class, IViewMapper<T, TORM> =>
                services
                    .AddScoped<TContext>()
                    .AddSame<IReadContext<T>, TContext>()
                    .AddScoped<IViewMapper<T, TORM>, TMapper>()
                    .AddSame<IReadRepository<TORM>, Repository<TORM>>();
            

    }
}