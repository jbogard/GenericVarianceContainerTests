namespace GenericVarianceContainerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.ServiceLocation;
    using Ninject.Components;
    using Ninject.Extensions.Conventions;
    using Ninject;
    using Ninject.Infrastructure;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;

    public class NinjectTests : TestBase
    {
        public NinjectTests()
        {
            var kernel = new StandardKernel();
            kernel.Bind(scan => scan.FromAssemblyContaining<A>().SelectAllClasses().BindAllInterfaces());

            ServiceLocator = new NinjectServiceLocator(kernel);
        }
    }

    public class NinjectServiceLocator : ServiceLocatorImplBase
    {
        public IKernel Kernel { get; private set; }

        public NinjectServiceLocator(IKernel kernel)
        {
            Kernel = kernel;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            // key == null must be specifically handled as not asking for a specific keyed instance
            // http://commonservicelocator.codeplex.com/wikipage?title=API%20Reference&referringTitle=Home
            //     The implementation should be designed to expect a null for the string key parameter, 
            //     and MUST interpret this as a request to get the "default" instance for the requested 
            //     type. This meaning of default varies from locator to locator.
            if (key == null)
            {
                return Kernel.Get(serviceType);
            }
            return Kernel.Get(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }
    }

    public class Ninject_Extended_Tests : TestBase
    {
        public Ninject_Extended_Tests()
        {
            var kernel = new StandardKernel();
            kernel.Components.Add<IBindingResolver, ContravariantBindingResolver>();
            kernel.Components.Add<IBindingResolver, CovariantBindingResolver>();
            kernel.Bind(scan => scan.FromAssemblyContaining<A>().SelectAllClasses().BindAllInterfaces());

            ServiceLocator = new NinjectServiceLocator(kernel);
        }

        public class ContravariantBindingResolver : NinjectComponent, IBindingResolver
        {
            /// <summary>
            /// Returns any bindings from the specified collection that match the specified service.
            /// </summary>
            public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
            {
                if (service.IsGenericType)
                {
                    var genericType = service.GetGenericTypeDefinition();
                    var genericArguments = genericType.GetGenericArguments();
                    if (genericArguments.Count() == 1
                     && genericArguments.Single().GenericParameterAttributes.HasFlag(GenericParameterAttributes.Contravariant))
                    {
                        var argument = service.GetGenericArguments().Single();
                        var matches = bindings.Where(kvp => kvp.Key.IsGenericType
                                                               && kvp.Key.GetGenericTypeDefinition().Equals(genericType)
                                                               && kvp.Key.GetGenericArguments().Single() != argument
                                                               && kvp.Key.GetGenericArguments().Single().IsAssignableFrom(argument))
                            .SelectMany(kvp => kvp.Value);
                        return matches;
                    }
                }

                return Enumerable.Empty<IBinding>();
            }
        }
        public class CovariantBindingResolver : NinjectComponent, IBindingResolver
        {
            /// <summary>
            /// Returns any bindings from the specified collection that match the specified service.
            /// </summary>
            public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
            {
                if (service.IsGenericType)
                {
                    var genericType = service.GetGenericTypeDefinition();
                    var genericArguments = genericType.GetGenericArguments();
                    if (genericArguments.Count() == 1
                     && genericArguments.Single().GenericParameterAttributes.HasFlag(GenericParameterAttributes.Covariant))
                    {
                        var argument = service.GetGenericArguments().Single();
                        return bindings.Where(kvp => kvp.Key.IsGenericType
                                                  && kvp.Key.GetGenericTypeDefinition().Equals(genericType)
                                                  && argument.IsAssignableFrom(kvp.Key.GetGenericArguments().Single()))
                                   .SelectMany(kvp => kvp.Value);
                    }
                }

                return Enumerable.Empty<IBinding>();
            }
        }
    }
}