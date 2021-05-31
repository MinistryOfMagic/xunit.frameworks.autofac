using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework
{
    internal class AutofacTheoryDiscoverer : TheoryDiscoverer
    {
        public AutofacTheoryDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink) { }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions,
                                                                                ITestMethod testMethod,
                                                                                IAttributeInfo theoryAttribute) =>
            new[] { new AutofacTheoryTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod) };

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions,
                                                                                 ITestMethod testMethod,
                                                                                 IAttributeInfo theoryAttribute,
                                                                                 object[] dataRow)
        {
            return new[] { new AutofacTestCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, dataRow) };
        }
    }
}

