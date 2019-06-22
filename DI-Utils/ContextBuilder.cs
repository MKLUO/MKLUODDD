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
                .AddSameScoped<IContextHandle, ContextHandle>()
                .AddSameScoped<IRegistableContextHandle, ContextHandle>();
            return services;
        }

        // Same instance used among multiple interfaces

        public static IServiceCollection AddSameScoped<IService, TService> (this IServiceCollection services) 
            where IService : class 
            where TService : class, IService => services.AddScoped<IService, TService>(x => x.GetRequiredService<TService>());


        // Repo => Context builder
        #region ContextBuilder

        public interface IServiceContextBuilder<TD> 
            where TD : class, new() {

            IServiceCollection Service { get; }

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

            public IServiceCollection Service { get; set; } = new ServiceCollection();

            public IServiceContextBuilder<TD> WithContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IContext<T>, IPersistContext<T>, IAggregationContext<T, TD>, IHookerContext<TD>
                where TMapper : class, IMapper<T, TD> =>                 
                new ServiceContextBuilder<TD>{ Service = Service.AddContext<T, TD, TContext, TMapper>() };

            public IServiceContextBuilder<TD> WithReadonlyContext<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TD>
                where TMapper : class, IReadonlyMapper<T, TD> =>                 
                new ServiceContextBuilder<TD>{ Service = Service.AddReadonlyContext<T, TD, TContext, TMapper>() };

            public IServiceContextBuilder<TD> WithView<T, TMapper, TContext>()
                where T : class
                where TContext : class, IReadContext<T>
                where TMapper : class, IViewMapper<T, TD> =>                 
                new ServiceContextBuilder<TD>{ Service = Service.AddView<T, TD, TContext, TMapper>() };
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
                    .AddSameScoped<IContext<T>, TContext>()
                    .AddSameScoped<IPersistContext<T>, TContext>()
                    .AddSameScoped<IAggregationContext<T, TD>, TContext>()
                    .AddSameScoped<IHookerContext<TD>, TContext>()

                    .AddScoped<IMapper<T, TD>, TMapper>()

                    .AddSameScoped<IWriteRepository<TD>, Repository<TD>>();

        public static IServiceCollection AddReadonlyContext<T, TD, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TD : class, new()
            where TContext : class, IReadContext<T>, IPersistContext<T>, IAggregationContext<T, TD>
            where TMapper : class, IReadonlyMapper<T, TD> =>
                services
                    .AddScoped<TContext>()
                    .AddSameScoped<IReadContext<T>, TContext>()
                    .AddSameScoped<IPersistContext<T>, TContext>()
                    .AddSameScoped<IAggregationContext<T, TD>, TContext>()

                    .AddScoped<IReadonlyMapper<T, TD>, TMapper>()

                    .AddSameScoped<IBaseRepository<TD>, Repository<TD>>();

        public static IServiceCollection AddView<T, TD, TContext, TMapper>(
            this IServiceCollection services)
            where T : class
            where TD : class, new()
            where TContext : class, IReadContext<T>
            where TMapper : class, IViewMapper<T, TD> =>
                services
                    .AddScoped<TContext>()
                    .AddSameScoped<IReadContext<T>, TContext>()

                    .AddScoped<IViewMapper<T, TD>, TMapper>()

                    .AddSameScoped<IReadRepository<TD>, Repository<TD>>();
            

    }
}