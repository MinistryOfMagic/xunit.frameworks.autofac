using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using Xunit.Abstractions;
using Xunit.Frameworks.Autofac.TestFramework;
using Xunit.Sdk;

namespace Xunit.Frameworks.Autofac;

public abstract class AutofacTestFramework : Sdk.TestFramework
{
    protected AutofacTestFramework(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo)
    {
        return new AutofacTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
    }

    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
    {
        return new AutofacTestFrameworkExecutor(assemblyName, CreateContainer(), SourceInformationProvider, DiagnosticMessageSink);
    }

    private IContainer CreateContainer()
    {
        ContainerBuilder builder = new();

        builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        builder.RegisterType<TestOutputHelper>().As<ITestOutputHelper>().AsSelf().InstancePerTest();

        ConfigureContainer(builder);

        return builder.Build();
    }

    protected abstract void ConfigureContainer(ContainerBuilder builder);
}
