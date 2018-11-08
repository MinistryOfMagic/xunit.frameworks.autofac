using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework
{
    public class AutofacTestCase : XunitTestCase
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public AutofacTestCase() { }

        public AutofacTestCase(IMessageSink diagnosticMessageSink,
                               TestMethodDisplay defaultMethodDisplay,
                               ITestMethod testMethod,
                               object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.All, testMethod, testMethodArguments) { }

        public ILifetimeScope TestClassLifetimeScope { get; set; }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                                  IMessageBus messageBus,
                                                  object[] constructorArguments,
                                                  ExceptionAggregator aggregator,
                                                  CancellationTokenSource cancellationTokenSource) =>
            new AutofacTestCaseRunner(this, TestClassLifetimeScope, DisplayName, SkipReason, constructorArguments, TestMethodArguments, messageBus, aggregator,
                                      cancellationTokenSource).RunAsync();
    }
}
