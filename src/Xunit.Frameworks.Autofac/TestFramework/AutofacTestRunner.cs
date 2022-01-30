using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

public class AutofacTestRunner : XunitTestRunner
{
    private readonly ILifetimeScope _testClassLifetimeScope;

    public AutofacTestRunner(ILifetimeScope testClassLifetimeScope,
                             ITest test,
                             IMessageBus messageBus,
                             Type testClass,
                             object[] constructorArguments,
                             MethodInfo testMethod,
                             object[] testMethodArguments,
                             string skipReason,
                             IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
                             ExceptionAggregator aggregator,
                             CancellationTokenSource cancellationTokenSource)
        : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator,
               cancellationTokenSource)
    {
        _testClassLifetimeScope = testClassLifetimeScope;
    }

    protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
    {
        await using ILifetimeScope testLifetimeScope =
            _testClassLifetimeScope.BeginLifetimeScope(AutofacTestScopes.Test, builder => builder.RegisterModules(TestClass));

        TestOutputHelper testOutputHelper = testLifetimeScope.Resolve<TestOutputHelper>();
        testOutputHelper.Initialize(MessageBus, Test);
        decimal seconds = await new AutofacTestInvoker(testLifetimeScope, Test, MessageBus, TestClass, ConstructorArguments, TestMethod,
                                                       TestMethodArguments,
                                                       BeforeAfterAttributes, aggregator, CancellationTokenSource).RunAsync();
        string output = testOutputHelper.Output;
        testOutputHelper.Uninitialize();
        return Tuple.Create(seconds, output);
    }
}
