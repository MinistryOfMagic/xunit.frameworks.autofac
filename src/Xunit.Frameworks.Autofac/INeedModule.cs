using Autofac;

namespace Xunit.Frameworks.Autofac;

// ReSharper disable once UnusedTypeParameter
public interface INeedModule<T> where T : Module, new()
{
}
