namespace GenericVarianceContainerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using StructureMap;

    public class StructureMapTests : TestBase
    {
        public StructureMapTests()
        {
            var container = new Container(cfg =>
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<A>();
                    scanner.WithDefaultConventions();
                    scanner.RegisterConcreteTypesAgainstTheFirstInterface();
                    scanner.AddAllTypesOf(typeof (B<>));
                    scanner.AddAllTypesOf(typeof (C<,>));
                    scanner.AddAllTypesOf(typeof(D<>));
                    scanner.AddAllTypesOf(typeof(E<,>));
                    scanner.AddAllTypesOf(typeof(F<,>));
                })
                );

            ServiceLocator = new StructureMapServiceLocator(container);
        }

        public class StructureMapServiceLocator : ServiceLocatorImplBase
        {
            private readonly IContainer _container;

            public StructureMapServiceLocator(IContainer container)
            {
                _container = container;
            }

            protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
            {
                return _container.GetAllInstances(serviceType).Cast<object>();
            }

            protected override object DoGetInstance(Type serviceType, string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return serviceType.IsAbstract || serviceType.IsInterface
                               ? _container.TryGetInstance(serviceType)
                               : _container.GetInstance(serviceType);
                }

                return _container.GetInstance(serviceType, key);
            }
        }

    }
}