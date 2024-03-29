namespace Xunit.Frameworks.Autofac;

/// <summary>
/// Used to decorate xUnit.net test classes and collections to indicate a test which has
/// per-theory fixture data. An instance of the fixture data is initialized just before
/// the first test in the theory is run, and if it implements IDisposable, is disposed
/// after the last test in the collection is run. To gain access to the fixture data from
/// inside the test, a constructor argument should be added to the test class which
/// exactly matches the <typeparamref name="TFixture" />.
/// </summary>
/// <typeparam name="TFixture">The type of the fixture.</typeparam>
/// <remarks>
/// If asynchronous setup of <typeparamref name="TFixture" /> is required
/// it should implement the <see cref="T:Xunit.IAsyncLifetime" /> interface.
/// </remarks>
public interface ITheoryFixture<TFixture> where TFixture : class
{

}
