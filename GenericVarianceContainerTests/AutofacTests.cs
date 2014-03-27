namespace GenericVarianceContainerTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Autofac.Features.Variance;
    using Microsoft.Practices.ServiceLocation;

    public class AutofacTests : TestBase
    {
        public AutofacTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(A).Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(A).Assembly).AsClosedTypesOf(typeof(B<>));
            builder.RegisterAssemblyTypes(typeof(A).Assembly).AsClosedTypesOf(typeof(D<>));
            builder.RegisterAssemblyTypes(typeof(A).Assembly).AsClosedTypesOf(typeof(E<,>));
            builder.RegisterAssemblyTypes(typeof(A).Assembly).AsClosedTypesOf(typeof(F<,>));

            ServiceLocator = new AutofacServiceLocator(builder.Build());
        }

        public sealed class AutofacServiceLocator : ServiceLocatorImplBase
        {
            readonly IComponentContext _container;

            public AutofacServiceLocator(IComponentContext container)
            {
                if (container == null)
                    throw new ArgumentNullException("container");
                _container = container;
            }

            protected override object DoGetInstance(Type serviceType, string key)
            {
                return key != null ? _container.ResolveNamed(key, serviceType) : _container.Resolve(serviceType);
            }

            protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
            {
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);

                object instance = _container.Resolve(enumerableType);
                return ((IEnumerable)instance).Cast<object>();
            }
        }

    }
}