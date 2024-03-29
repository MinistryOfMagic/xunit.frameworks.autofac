using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

internal class AutofacFactDiscoverer : FactDiscoverer
{
    public AutofacFactDiscoverer(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    protected override IXunitTestCase
        CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
    {
        return new AutofacTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod);
    }
}
