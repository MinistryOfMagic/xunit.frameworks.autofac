using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

internal class AutofacTheoryTestCase : AutofacTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    [UsedImplicitly]
    public AutofacTheoryTestCase()
    {
    }

    public AutofacTheoryTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod,
                                 object[] testMethodArguments = null)
        : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
    {
    }

    public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                              IMessageBus messageBus,
                                              object[] constructorArguments,
                                              ExceptionAggregator aggregator,
                                              CancellationTokenSource cancellationTokenSource)
    {
        return new AutofacTheoryTestCaseRunner(this, TestCaseLifetimeScope, DisplayName, SkipReason, constructorArguments, diagnosticMessageSink,
                                               messageBus,
                                               aggregator, cancellationTokenSource).RunAsync();
    }
}
