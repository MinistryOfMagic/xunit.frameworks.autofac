using System.Reflection;
using Autofac;
using Autofac.Features.ResolveAnything;
using Xunit.Abstractions;
using Xunit.Frameworks.Autofac.TestFramework;

namespace Xunit.Frameworks.Autofac
{
    public abstract class AutofacTestFramework : Sdk.TestFramework
    {
        protected AutofacTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink) { }
        
        protected override ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assemblyInfo) =>
            new AutofacTestFrameworkDiscoverer(assemblyInfo, SourceInformationProvider, DiagnosticMessageSink);

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
            new AutofacTestFrameworkExecutor(assemblyName, CreateContainer(), SourceInformationProvider, DiagnosticMessageSink);

        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            ConfigureContainer(builder);

            return builder.Build();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);
    }
}
