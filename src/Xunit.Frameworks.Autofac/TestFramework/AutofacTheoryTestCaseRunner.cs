using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Autofac;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

internal class AutofacTheoryTestCaseRunner : XunitTheoryTestCaseRunner
{
    private readonly ILifetimeScope _testRunnerLifetimeScope;

    public AutofacTheoryTestCaseRunner(IXunitTestCase testCase,
                                       ILifetimeScope testRunnerLifetimeScope,
                                       string displayName,
                                       string skipReason,
                                       object[] constructorArguments,
                                       IMessageSink diagnosticMessageSink,
                                       IMessageBus messageBus,
                                       ExceptionAggregator aggregator,
                                       CancellationTokenSource cancellationTokenSource)
        : base(testCase, displayName, skipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource)
    {
        _testRunnerLifetimeScope = testRunnerLifetimeScope;
    }

    protected override XunitTestRunner CreateTestRunner(ITest test,
                                                        IMessageBus messageBus,
                                                        Type testClass,
                                                        object[] constructorArguments,
                                                        MethodInfo testMethod,
                                                        object[] testMethodArguments,
                                                        string skipReason,
                                                        IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
                                                        ExceptionAggregator aggregator,
                                                        CancellationTokenSource cancellationTokenSource)
    {
        return new AutofacTestRunner(
            _testRunnerLifetimeScope, test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes,
            new ExceptionAggregator(aggregator), cancellationTokenSource);
    }

    protected override ITest CreateTest(IXunitTestCase testCase, string displayName)
    {
        return new AutofacTest(_testRunnerLifetimeScope, testCase, displayName);
    }
}
