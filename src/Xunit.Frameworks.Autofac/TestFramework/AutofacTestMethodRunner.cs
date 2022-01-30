using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

public class AutofacTestMethodRunner : XunitTestMethodRunner
{
    private readonly IMessageSink _diagnosticMessageSink;

    private readonly ILifetimeScope _testMethodLifetimeScope;

    public AutofacTestMethodRunner(ILifetimeScope testMethodLifetimeScope,
                                   IMessageSink diagnosticMessageSink,
                                   ITestMethod testMethod,
                                   IReflectionTypeInfo @class,
                                   IReflectionMethodInfo method,
                                   IEnumerable<IXunitTestCase> testCases,
                                   IMessageBus messageBus,
                                   ExceptionAggregator aggregator,
                                   CancellationTokenSource cancellationTokenSource,
                                   object[] constructorArguments)
        : base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
    {
        _testMethodLifetimeScope = testMethodLifetimeScope;
        _diagnosticMessageSink = diagnosticMessageSink;
    }

    protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
    {
        if (testCase is AutofacTestCase autofacTestCase) autofacTestCase.TestCaseLifetimeScope = _testMethodLifetimeScope;

        return await testCase.RunAsync(_diagnosticMessageSink, MessageBus, new object[] { }, Aggregator, CancellationTokenSource);
    }
}
