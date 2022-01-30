using Autofac;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

[assembly: TestFramework("Xunit.Frameworks.Autofac.Tests.TestFramework", "Xunit.Frameworks.Autofac.Tests")]

namespace Xunit.Frameworks.Autofac.Tests;

[UsedImplicitly]
public class TestFramework : AutofacTestFramework
{
    public TestFramework(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    protected override void ConfigureContainer(ContainerBuilder builder)
    {
    }
}
