namespace GenericVarianceContainerTests
{
    using System.Linq;
    using Microsoft.Practices.ServiceLocation;
    using Shouldly;

    public abstract class TestBase
    {
        protected IServiceLocator ServiceLocator;

        public void Should_resolve_A()
        {
            var instance = ServiceLocator.GetInstance<A>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<AImpl>();
        }

        public void Should_resolve_B()
        {
            var instance = ServiceLocator.GetInstance<B<R>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<BImpl>();
        }

        public void Should_resolve_B_contravariant()
        {
            var instance = ServiceLocator.GetInstance<B<U>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<BImpl>();
        }

        public void Should_resolve_C()
        {
            var instances = ServiceLocator.GetAllInstances<C<R, S>>();
            instances.Count().ShouldBe(2);
            instances.OfType<CImpl>().Count().ShouldBe(1);
            instances.OfType<CAbsImpl>().Count().ShouldBe(1);
        }

        public void Should_resolve_C_contravariant()
        {
            var instances = ServiceLocator.GetAllInstances<C<U, W>>();
            instances.Count().ShouldBe(2);
            instances.OfType<CImpl>().Count().ShouldBe(1);
            instances.OfType<CAbsImpl>().Count().ShouldBe(1);
        }


        public void Should_resolve_D()
        {
            var instance = ServiceLocator.GetInstance<D<U>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<DImpl>();
        }

        public void Should_resolve_D_covariant()
        {
            var instance = ServiceLocator.GetInstance<D<R>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<DImpl>();
        }

        public void Should_resolve_E()
        {
            var instance = ServiceLocator.GetInstance<E<U, W>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<EImpl>();
        }

        public void Should_resolve_E_covariant()
        {
            var instance = ServiceLocator.GetInstance<E<R, S>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<EImpl>();
        }

        public void Should_resolve_F()
        {
            var instance = ServiceLocator.GetInstance<F<R, W>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<FImpl>();
        }

        public void Should_resolve_F_co_and_contravariant()
        {
            var instance = ServiceLocator.GetInstance<F<U, S>>();
            instance.ShouldNotBe(null);
            instance.ShouldBeOfType<FImpl>();
        }
    }
}