xUnit Autofac
=============

Use Autofac to resolve xUnit test cases.

The Test runners and discoverers are based on their xUnit counterparts. If `[UseAutofacTestFramework]` is missing, the tests in that class are run by the normal xUnit runners.

Originally a fork of [xunit.ioc.autofac] by @dennisroche

No longer maintained
====================

**This project is no longer maintained**. We suggest that you migrate your existing solutions to use [Xunit.DependencyInjection](https://github.com/pengweiqhca/Xunit.DependencyInjection) instead. For example:

1. Replace `ConfigureTestFramework` with `Startup`, and make sure that you specify the `ConfigureHost` method and `UseServiceProviderFactory` to specify that you want to use Autofac.
Remove the `TestFramework` attribute.

    ```diff
    diff --git a/ConfigureTestFramework.cs b/ConfigureTestFramework.cs
    index efad236..187e43c 100644
    --- a/ConfigureTestFramework.cs
    +++ b/ConfigureTestFramework.cs
    @@ -1,23 +1,34 @@
    -[assembly: TestFramework("Your.Test.Project.ConfigureTestFramework", "AssemblyName")]
    -
    namespace Your.Test.Project
    {
    -    public class ConfigureTestFramework : AutofacTestFramework
    +    public class Startup
        {
    -        public ConfigureTestFramework(IMessageSink diagnosticMessageSink)
    -            : base(diagnosticMessageSink)
    +        public void Configure(ILoggerFactory loggerFactory, XunitTestOutputLoggerProvider loggerProvider)
            {
    +            loggerFactory.AddProvider(loggerProvider);
            }

    -        protected override void ConfigureContainer(ContainerBuilder builder)
    +        public void ConfigureHost(IHostBuilder hostBuilder)
            {
    -            builder.RegisterType<CurrentTestInfo>().As<ICurrentTestInfo>().InstancePerTest();
    -            builder.RegisterType<CurrentTestClassInfo>().As<ICurrentTestClassInfo>().InstancePerTestClass();
    -            builder.RegisterType<CurrentTestCollectionInfo>().As<ICurrentTestCollectionInfo>().InstancePerTestCollection();
    +            hostBuilder
    +                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    +                .ConfigureContainer<ContainerBuilder>(RegisterDependencies);
    +        }

    -            builder.RegisterSource(new NSubstituteRegistrationSource()); // https://gist.github.com/dabide/57c5279894383d8f0ee4ed2069773907
    +        public void ConfigureServices(IServiceCollection serviceCollection)
    +        {
    +            serviceCollection.AddTransient<IFoo, Foo>();
    +        }

    -            builder.RegisterType<Foo>().As<IFoo>();
    +        private void RegisterDependencies(ContainerBuilder builder)
    +        {
    +            // Unfortunately test lifetime scopes will not work out-of-the-box.
    +            // It may be possible to add some test or test class lifetime support; see https://github.com/pengweiqhca/Xunit.DependencyInjection/blob/master/Xunit.DependencyInjection/IXunitTestCaseRunnerAdapter.cs
    +
    +            // builder.RegisterType<CurrentTestInfo>().As<ICurrentTestInfo>().InstancePerTest();
    +            // builder.RegisterType<CurrentTestClassInfo>().As<ICurrentTestClassInfo>().InstancePerTestClass();
    +            // builder.RegisterType<CurrentTestCollectionInfo>().As<ICurrentTestCollectionInfo>().InstancePerTestCollection();
    +
    +            builder.RegisterSource(new NSubstituteRegistrationSource()); // https://gist.github.com/dabide/57c5279894383d8f0ee4ed2069773907

                // configure your container
                // e.g. builder.RegisterModule<TestOverrideModule>();
    ```

2. Remove `UseAutofacTestFramework` from all of your test classes.

    ```diff
    diff --git a/EvenMoreAwesomeTests.cs b/EvenMoreAwesomeTests.cs
    index 266a806..6af55e5 100644
    --- a/EvenMoreAwesomeTests.cs
    +++ b/EvenMoreAwesomeTests.cs
    @@ -1,4 +1,3 @@
    -[UseAutofacTestFramework]
    public class MyEvenMoreAwesomeTests : IUseInMemoryDb
    {
        public MyEvenMoreAwesomeTests(IDbConnectionFactory dbConnectionFactory)
    diff --git a/MyAwesomeTests.cs b/MyAwesomeTests.cs
    index d078b66..035f40e 100644
    --- a/MyAwesomeTests.cs
    +++ b/MyAwesomeTests.cs
    @@ -1,4 +1,3 @@
    -[UseAutofacTestFramework] // Without this attribute, the test class will be handled by the standard xUnit test runners
    public class MyAwesomeTests
    {
        public MyAwesomeTests(IFoo foo)
    ```

How to use
----------

Install the [Nuget](https://www.nuget.org/packages/xunit.frameworks.autofac) package.

    Install-Package xunit.frameworks.autofac

In your testing project, add the following framework

```cs
[assembly: TestFramework("Your.Test.Project.ConfigureTestFramework", "AssemblyName")]

namespace Your.Test.Project
{
    public class ConfigureTestFramework : AutofacTestFramework
    {
        public ConfigureTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<CurrentTestInfo>().As<ICurrentTestInfo>().InstancePerTest();
            builder.RegisterType<CurrentTestClassInfo>().As<ICurrentTestClassInfo>().InstancePerTestClass();
            builder.RegisterType<CurrentTestCollectionInfo>().As<ICurrentTestCollectionInfo>().InstancePerTestCollection();

            builder.RegisterSource(new NSubstituteRegistrationSource()); // https://gist.github.com/dabide/57c5279894383d8f0ee4ed2069773907

            builder.RegisterType<Foo>().As<IFoo>();

            // configure your container
            // e.g. builder.RegisterModule<TestOverrideModule>();
        }
    }
}
```

Example test `class`.

```cs
[UseAutofacTestFramework] // Without this attribute, the test class will be handled by the standard xUnit test runners
public class MyAwesomeTests
{
    public MyAwesomeTests(IFoo foo)
    {
        _foo = foo;
    }

    [Fact]
    public void AssertThatWeDoStuff()
    {
        Console.WriteLine(_foo.Bar);
    }

    private readonly ITestOutputHelper _outputHelper;
}

public interface IFoo
{
    Guid Bar { get; }
}

public class Foo : IFoo
{
    public Guid Bar { get; } = Guid.NewGuid();
}
```

`ICollectionFixture<T>` and `IClassFixture<T>` are also supported, together with `INeedModule<T>`. (The latter specifies Autofac modules to be loaded when the lifetime scope is created.) This enables very elegant solutions:

```cs
[UseAutofacTestFramework]
public class MyEvenMoreAwesomeTests : IUseInMemoryDb
{
    public MyEvenMoreAwesomeTests(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    [Fact]
    public void AssertThatWeDoEvenMoreStuff()
    {
        using (IDbConnection db = _dbConnectionFactory.Open())
        {
            db.CreateTableIfNotExists<Foo>();
            // ... and so on
        }
    }

    private readonly IDbConnectionFactory _dbConnectionFactory;
}

public interface IUseInMemoryDb : IClassFixture<MemoryDatabaseClassFixture>
{
}

public class MemoryDatabaseClassFixture : IDisposable, INeedModule<MemoryDatabaseClassFixture.MemoryDatabaseFixtureModule>
{
    private readonly IDbConnection _db;

    public MemoryDatabaseClassFixture(IDbConnectionFactory dbConnectionFactory)
    {
        // Keep the in-memory database alive
        _db = dbConnectionFactory.Open();
    }

    public void Dispose()
    {
        // Now it can rest in peace
        _db?.Dispose();
    }

    public class MemoryDatabaseFixtureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider)).As<IDbConnectionFactory>().SingleInstance();
        }
    }
}

```

License
=======

MIT
