namespace GenericVarianceContainerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using SimpleInjector;
    using SimpleInjector.Extensions;

    public class SimpleInjectorTests : TestBase
    {
        public SimpleInjectorTests()
        {
            var container = new Container();
            
            container.Register<A, AImpl>();
            container.RegisterManyForOpenGeneric(typeof(B<>), typeof(A).Assembly);
            container.RegisterManyForOpenGeneric(typeof(C<,>), ((t, impls) =>
            {
                foreach (var impl in impls.Where(impl => !impl.IsAbstract))
                {
                    container.Register(impl);
                }
            }), typeof(A).Assembly);
            container.RegisterManyForOpenGeneric(typeof(D<>), typeof(A).Assembly);
            container.RegisterManyForOpenGeneric(typeof(E<,>), typeof(A).Assembly);
            container.RegisterManyForOpenGeneric(typeof(F<,>), typeof(A).Assembly);
            AllowToResolveVariantCollections(container);

            ServiceLocator = new SimpleInjectorServiceLocatorAdapter(container);
        }

        public static void AllowToResolveVariantCollections(Container container)
        {
            container.ResolveUnregisteredType += (sender, e) =>
            {
                // Only handle IEnumerable<T>.
                if (!e.UnregisteredServiceType.IsGenericType ||
                    e.UnregisteredServiceType.GetGenericTypeDefinition() !=
                        typeof(IEnumerable<>))
                {
                    return;
                }

                Type serviceType =
                    e.UnregisteredServiceType.GetGenericArguments()[0];

                if (!serviceType.IsGenericType)
                {
                    return;
                }

                Type def = serviceType.GetGenericTypeDefinition();

                var registrations = (
                    from r in container.GetCurrentRegistrations()
                    where r.ServiceType.IsGenericType
                    where r.ServiceType.GetGenericTypeDefinition() == def
                    where serviceType.IsAssignableFrom(r.ServiceType)
                    select r)
                    .ToArray();

                if (registrations.Any())
                {
                    var instances =
                        registrations.Select(r => r.GetInstance());

                    var castMethod = typeof(Enumerable).GetMethod("Cast")
                        .MakeGenericMethod(serviceType);

                    var castedInstances =
                        castMethod.Invoke(null, new[] { instances });

                    e.Register(() => castedInstances);
                }
            };
        }

        public class SimpleInjectorServiceLocatorAdapter : ServiceLocatorImplBase
        {
            private readonly Container _container;

            public SimpleInjectorServiceLocatorAdapter(Container container)
            {
                _container = container;
            }

            protected override object DoGetInstance(Type serviceType, string key)
            {
                return _container.GetInstance(serviceType);
            }

            protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
            {
                return _container.GetAllInstances(serviceType);
            }
        }
    }


}