using Xunit.Abstractions;

namespace Xunit.Frameworks.Autofac.Tests;

[UseAutofacTestFramework]
public class TestOutputHelperTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TestOutputHelperTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Outputs_foo()
    {
        _testOutputHelper.WriteLine("Foo");
    }

    [Fact]
    public void Outputs_bar()
    {
        _testOutputHelper.WriteLine("bar");
    }
}
