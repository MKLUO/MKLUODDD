using System;
using Microsoft.Extensions.DependencyInjection;

namespace MKLUODDD.Context {

    public class AggregationContextProxy<T, TD> : IAggregationContextProxy<T, TD> where TD : class {

        IAggregationContext<T, TD> ? Context { get; set; } = null;

        IServiceProvider ServiceProvider { get; }
        public AggregationContextProxy(IServiceProvider serviceProvider) =>
            this.ServiceProvider = serviceProvider;        

        void Init() {
            if (Context == null)
                Context = ServiceProvider.GetRequiredService<IAggregationContext<T, TD>>();
        }

        public T Pull(in TD obj, bool halt = false) {
            if (Context == null) Init();
            if (Context == null) throw new AggregationContextProxtInitializationException();
            return Context.Pull(obj, halt);
        }

        public TD? Push(in T entity) {
            if (Context == null) Init();
            if (Context == null) throw new AggregationContextProxtInitializationException();
            return Context.Push(entity);
        }

        public TD PushTransient(in T entity) {
            if (Context == null) Init();
            if (Context == null) throw new AggregationContextProxtInitializationException();
            return Context.PushTransient(entity);
        }

        public bool IsTransient(in T entity) {
            if (Context == null) Init();
            if (Context == null) throw new AggregationContextProxtInitializationException();
            return Context.IsTransient(entity);
        }

        public void AttachTransient(in T entity, in TD obj) {
            if (Context == null) Init();
            if (Context == null) throw new AggregationContextProxtInitializationException();
            Context.AttachTransient(entity, obj);
        }

        [Serializable]
        public class AggregationContextProxtInitializationException : System.Exception
        {
            public AggregationContextProxtInitializationException() { }
            public AggregationContextProxtInitializationException(string message) : base(message) { }
            public AggregationContextProxtInitializationException(string message, System.Exception inner) : base(message, inner) { }
            protected AggregationContextProxtInitializationException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}