using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

public class AutofacTestFrameworkDiscoverer : XunitTestFrameworkDiscoverer
{
    private readonly AutofacFactDiscoverer _autofacFactDiscoverer;
    private readonly AutofacTheoryDiscoverer _autofacTheoryDiscoverer;

    public AutofacTestFrameworkDiscoverer(IAssemblyInfo assemblyInfo, ISourceInformationProvider sourceProvider, IMessageSink diagnosticMessageSink)
        : base(assemblyInfo, sourceProvider, diagnosticMessageSink)
    {
        _autofacFactDiscoverer = new AutofacFactDiscoverer(diagnosticMessageSink);
        _autofacTheoryDiscoverer = new AutofacTheoryDiscoverer(diagnosticMessageSink);
    }

    protected override bool FindTestsForType(ITestClass testClass,
                                             bool includeSourceInformation,
                                             IMessageBus messageBus,
                                             ITestFrameworkDiscoveryOptions discoveryOptions)
    {
        bool hasAttribute = testClass.Class.GetCustomAttributes(typeof(UseAutofacTestFrameworkAttribute)).Any();
        if (!hasAttribute) return base.FindTestsForType(testClass, includeSourceInformation, messageBus, discoveryOptions);

        foreach (IMethodInfo method in testClass.Class.GetMethods(true))
        {
            TestMethod testMethod = new(testClass, method);
            if (!FindTestsForMethodOnAutofacTestClass(testMethod, includeSourceInformation, messageBus, discoveryOptions)) return false;
        }

        return true;
    }

    private bool FindTestsForMethodOnAutofacTestClass(ITestMethod testMethod,
                                                      bool includeSourceInformation,
                                                      IMessageBus messageBus,
                                                      ITestFrameworkDiscoveryOptions discoveryOptions)
    {
        List<IAttributeInfo> factAttributes = testMethod.Method.GetCustomAttributes(typeof(FactAttribute)).ToList();
        if (factAttributes.Count > 1)
        {
            string message = $"Test method '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}' has multiple [Fact]-derived attributes";
            ExecutionErrorTestCase testCase = new(DiagnosticMessageSink, TestMethodDisplay.ClassAndMethod, TestMethodDisplayOptions.All, testMethod, message);
            return ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus);
        }

        IAttributeInfo factAttribute = factAttributes.FirstOrDefault();

        if (factAttribute == null) return true;

        Type factAttributeType = (factAttribute as IReflectionAttributeInfo)?.Attribute.GetType();

        IXunitTestCaseDiscoverer discoverer = null;
        if (factAttributeType == typeof(FactAttribute))
        {
            discoverer = _autofacFactDiscoverer;
        }
        else if (factAttributeType == typeof(TheoryAttribute)) discoverer = _autofacTheoryDiscoverer;

        if (discoverer == null) return true;

        foreach (IXunitTestCase testCase in discoverer.Discover(discoveryOptions, testMethod, factAttribute))
        {
            if (!ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus)) return false;
        }

        return true;
    }
}
