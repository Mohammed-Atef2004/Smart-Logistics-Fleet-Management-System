using Domain.Fleet;
using NetArchTest.Rules;

namespace ArchitectureTest
{
    public class UnitTest1
    {
        [Fact]
        public void Domain_Should_Not_Have_Dependencies_On_Other_Projects()
        {
            // ?????? 'User' ??? ???? ????? ????? ??? ????? ??? Domain ????
            var assembly = typeof(Domain.Entities.User).Assembly;

            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(
                    InfrastructureNamespace,
                    ApplicationNamespace,
                    PresentationNamespace)
                .GetResult();

            Assert.True(result.IsSuccessful, "Domain layer should not depend on other layers.");
        }
    }
}