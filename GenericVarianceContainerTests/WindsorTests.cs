namespace GenericVarianceContainerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;

    public class WindsorTests : TestBase
    {
        public WindsorTests()
        {
            var container = new WindsorContainer();
            container.Register(Classes.FromAssemblyContaining<A>().Pick().WithServiceAllInterfaces());

            ServiceLocator = new WindsorServiceLocator(container);
        }
    }

    public class Windsor_Extended_Tests : TestBase
    {
        public Windsor_Extended_Tests()
        {
            var container = new WindsorContainer();
            container.Kernel.AddHandlersFilter(new ContravariantFilter());
            container.Register(Classes.FromAssemblyContaining<A>().Pick().WithServiceAllInterfaces());

            ServiceLocator = new WindsorServiceLocator(container);
        }

        public class ContravariantFilter : IHandlersFilter
        {
            public bool HasOpinionAbout(Type service)
            {
                if (!service.IsGenericType)
                    return false;

                var genericType = service.GetGenericTypeDefinition();
                var genericArguments = genericType.GetGenericArguments();
                return genericArguments.All(t => t.GenericParameterAttributes.HasFlag(GenericParameterAttributes.Contravariant));
            }

            public IHandler[] SelectHandlers(Type service, IHandler[] handlers)
            {
                return handlers;
            }
        }
    }

    public class WindsorServiceLocator : ServiceLocatorImplBase
    {
        private readonly IWindsorContainer _container;

        public WindsorServiceLocator(IWindsorContainer container)
        {
            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
                return _container.Resolve(key, serviceType);
            return _container.Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return (object[])_container.ResolveAll(serviceType);
        }
    }
}
