using System.Reflection;
using Autofac;
using FluentAssertions;
using Module = Autofac.Module;

namespace Xunit.Frameworks.Autofac.Tests
{
    [UseAutofacTestFramework]
    [Collection("Foo")]
    public class CollectionFixtureTests
    {
        public CollectionFixtureTests(ILifetimeScope lifetimeScope, IFoo foo)
        {
            _lifetimeScope = lifetimeScope;
            _foo = foo;
        }

        private readonly ILifetimeScope _lifetimeScope;
        private readonly IFoo _foo;

        [Fact]
        public void Collections_work_correctly()
        {
            _foo.Should().NotBeNull();
            _lifetimeScope.Resolve<FooClassFixture>().Should().NotBeNull();
            _lifetimeScope.Resolve<FooCollectionFixture>().Should().NotBeNull();
        }

        [Theory]
        [InlineData("ABC-02-04-CDE")]
        [InlineData("ABC-02-40-CDE")]
        [InlineData("ABC-02-0040-CDE")]
        public void Theories_work_correctly(string input)
        {
            input.Should().Be(input);
        }

        private static int _numRuns = 0;

        [Theory]
        [InlineData("ABC-02-04-CDE")]
        [InlineData("ABC-02-40-CDE")]
        [InlineData("ABC-02-0040-CDE")]
        public void Theories_run_correct_num_times(string input)
        {
            var thisTestMethod = MethodInfo.GetCurrentMethod();
            var inlineDataAttributes = thisTestMethod.GetCustomAttributes(typeof(InlineDataAttribute), false);
            var numInlineDataItems = inlineDataAttributes.Length;

            input.Should().Be(input);

            _numRuns++;
            Assert.True(_numRuns <= numInlineDataItems,
                        $"Theory should only run {numInlineDataItems} times. It has run {_numRuns} times now.");
        }
    }


    [CollectionDefinition("Foo")]
    public class FooCollectionDefinition : ICollectionFixture<FooCollectionFixture>, IClassFixture<FooClassFixture> { }

    public class FooCollectionFixture : INeedModule<FooModule> { }

    public class FooClassFixture { }

    public class FooModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Foo>().As<IFoo>();
        }
    }

    public interface IFoo { }

    internal class Foo : IFoo { }
}
