namespace GenericVarianceContainerTests
{
    using Microsoft.Practices.Unity;

    public class UnityTests : TestBase
    {
        public UnityTests()
        {
            var container = new UnityContainer();
            container.RegisterTypes(AllClasses.FromAssemblies(typeof(A).Assembly), WithMappings.FromAllInterfaces, overwriteExistingMappings: true);

            ServiceLocator = new UnityServiceLocator(container);
        }
    }
}