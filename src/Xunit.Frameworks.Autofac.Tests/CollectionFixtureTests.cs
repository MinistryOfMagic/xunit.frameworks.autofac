using System.Collections.Generic;
using Autofac;
using FluentAssertions;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Xunit.Frameworks.Autofac.Tests;

[UseAutofacTestFramework]
[Collection("Foo")]
public class CollectionFixtureTests : ITheoryFixture<FooTheoryFixture>
{
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
            new object[] { 1 },
            new object[] { 2 },
            new object[] { 3 }
        };

    [Fact]
    public void Collections_work_correctly()
    {
        _foo.Should().NotBeNull();
        _lifetimeScope.Resolve<FooClassFixture>().Should().NotBeNull();
        _lifetimeScope.Resolve<FooCollectionFixture>().Should().NotBeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Theory_injection_scope_works_correctly_with_theory_fixtures(int number)
    {
        number.Should().Be(number);
        FooTheoryFixture _ = _lifetimeScope.Resolve<FooTheoryFixture>();
        FooTheoryFixture.InstantiationCount.Should().Be(1, "the injected instance should be the same for all tests in the theory");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Theory_injection_scope_works_correctly_with_modules(int number)
    {
        number.Should().Be(number);
        TheoryRunCounter _ = _lifetimeScope.Resolve<TheoryRunCounter>();
        TheoryRunCounter.InstantiationCount.Should().Be(1, "the injected instance should be the same for all tests in the theory");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Theories_with_InlineData_run_correct_num_times(int testId)
    {
        RunCounter runCounter = _lifetimeScope.Resolve<RunCounter>();

        runCounter.HasRun(testId);
        runCounter.GetRuns(testId).Should().Be(1, "a theory test case should only run once");
    }

    [Theory]
    [MemberData(nameof(TheoryTestData))]
    public void Theories_with_MemberData_run_correct_num_times(int testId)
    {
        RunCounter runCounter = _lifetimeScope.Resolve<RunCounter>();

        runCounter.HasRun(testId);
        runCounter.GetRuns(testId).Should().Be(1, "a theory test case should only run once");
    }
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class FooTheoryFixture
{
    public FooTheoryFixture()
    {
        InstantiationCount++;
    }

    public static int InstantiationCount { get; set; }
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class TheoryRunCounter
{
    public TheoryRunCounter()
    {
        InstantiationCount++;
    }

    public static int InstantiationCount { get; set; }
}

public class RunCounter
{
    private readonly Dictionary<int, int> _runs = new();

    public void HasRun(int testId)
    {
        if (!_runs.ContainsKey(testId))
        {
            _runs.Add(testId, 0);
        }

        _runs[testId]++;
    }

    public int GetRuns(int testId)
    {
        if (!_runs.ContainsKey(testId))
        {
            _runs.Add(testId, 0);
        }

        return _runs[testId];
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
        builder.RegisterType<RunCounter>().AsSelf().InstancePerTheory();
        builder.RegisterType<TheoryRunCounter>().AsSelf().InstancePerTheory();
    }
}

public interface IFoo
{
}

internal class Foo : IFoo
{
}
