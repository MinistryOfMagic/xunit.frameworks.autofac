using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using FluentAssertions;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Xunit.Frameworks.Autofac.Tests;

[UseAutofacTestFramework]
[Collection("Foo")]
public class CollectionFixtureTests
{
    private static int _numInlineDataTheoryRuns;

    private static int _numMemberDataTheoryRuns;
    private readonly IFoo _foo;

    private readonly ILifetimeScope _lifetimeScope;

    public CollectionFixtureTests(ILifetimeScope lifetimeScope, IFoo foo)
    {
        _lifetimeScope = lifetimeScope;
        _foo = foo;
    }

    public static IEnumerable<object[]> TheoryTestData =>
        new List<object[]>
        {
            new object[] { "Data 1" },
            new object[] { "Data 2" },
            new object[] { "Data 3" }
        };

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

    [Theory]
    [InlineData("ABC-02-04-CDE")]
    [InlineData("ABC-02-40-CDE")]
    [InlineData("ABC-02-0040-CDE")]
    public void Theories_with_InlineData_run_correct_num_times(string input)
    {
        MethodBase thisTestMethod = MethodBase.GetCurrentMethod();
        Debug.Assert(thisTestMethod != null, nameof(thisTestMethod) + " != null");
        object[] inlineDataAttributes = thisTestMethod.GetCustomAttributes(typeof(InlineDataAttribute), false);
        int numInlineDataItems = inlineDataAttributes.Length;

        input.Should().Be(input);

        _numInlineDataTheoryRuns++;
        Assert.True(_numInlineDataTheoryRuns <= numInlineDataItems,
                    $"Theory should only run {numInlineDataItems} times. It has run {_numInlineDataTheoryRuns} times now.");
    }

    [Theory]
    [MemberData(nameof(TheoryTestData))]
    public void Theories_with_MemberData_run_correct_num_times(string input)
    {
        int numMemberDataItems = TheoryTestData.Count();
        input.Should().Be(input);
        _numMemberDataTheoryRuns++;
        Assert.True(_numMemberDataTheoryRuns <= numMemberDataItems,
                    $"Theory should only run {numMemberDataItems} times. It has run {_numMemberDataTheoryRuns} times now.");
    }
}

[CollectionDefinition("Foo")]
public class FooCollectionDefinition : ICollectionFixture<FooCollectionFixture>, IClassFixture<FooClassFixture>
{
}

[UsedImplicitly]
public class FooCollectionFixture : INeedModule<FooModule>
{
}

[UsedImplicitly]
public class FooClassFixture
{
}

public class FooModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Foo>().As<IFoo>();
    }
}

public interface IFoo
{
}

internal class Foo : IFoo
{
}
