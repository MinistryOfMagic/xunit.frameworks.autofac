using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac.TestFramework;

public class AutofacTestClassRunner : XunitTestClassRunner
{
    private readonly ILifetimeScope _testRunnerLifetimeScope;

    public AutofacTestClassRunner(ILifetimeScope testRunnerLifetimeScope,
                                  ITestClass testClass,
                                  IReflectionTypeInfo @class,
                                  IEnumerable<IXunitTestCase> testCases,
                                  IMessageSink diagnosticMessageSink,
                                  IMessageBus messageBus,
                                  ITestCaseOrderer testCaseOrderer,
                                  ExceptionAggregator aggregator,
                                  CancellationTokenSource cancellationTokenSource,
                                  Dictionary<Type, object> collectionFixtureMappings)
        : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource,
               collectionFixtureMappings)
    {
        _testRunnerLifetimeScope = testRunnerLifetimeScope;
    }

    protected override void CreateClassFixture(Type fixtureType)
    {
        Aggregator.Run(() => { ClassFixtureMappings[fixtureType] = _testRunnerLifetimeScope.Resolve(fixtureType); });
    }

    protected override async Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod,
                                                                 IReflectionMethodInfo method,
                                                                 IEnumerable<IXunitTestCase> testCases,
                                                                 object[] constructorArguments)
    {
        await using ILifetimeScope theoryLifetimeScope = _testRunnerLifetimeScope.BeginLifetimeScope(AutofacTestScopes.Theory, builder => builder.RegisterTheoryFixturesAndModules(testMethod.TestClass, Class));
        return await new AutofacTestMethodRunner(theoryLifetimeScope, DiagnosticMessageSink, testMethod, Class, method, testCases, MessageBus,
                                                 new ExceptionAggregator(Aggregator), CancellationTokenSource, constructorArguments).RunAsync();
    }

    protected override object[] CreateTestClassConstructorArguments()
    {
        return Array.Empty<object>();
    }
 }
