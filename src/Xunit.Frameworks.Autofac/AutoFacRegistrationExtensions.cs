using System;
using Autofac.Builder;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Xunit.Frameworks.Autofac;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class AutoFacXUnitRegistrationExtensions
{
    /// <summary>
    ///     Registers a component so all dependant components will resolve the same shared instance within the theory
    ///     lifetime scope.
    /// </summary>
    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InstancePerTheory<TLimit, TActivatorData, TStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
    {
        if (registration == null) throw new ArgumentNullException(nameof(registration));

        return registration.InstancePerMatchingLifetimeScope(AutofacTestScopes.Theory);
    }

    /// <summary>
    ///     Registers a component so all dependant components will resolve the same shared instance within the test
    ///     lifetime scope.
    /// </summary>
    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InstancePerTest<TLimit, TActivatorData, TStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
    {
        if (registration == null) throw new ArgumentNullException(nameof(registration));

        return registration.InstancePerMatchingLifetimeScope(AutofacTestScopes.Test);
    }

    /// <summary>
    ///     Registers a component so all dependant components will resolve the same shared instance within the test class
    ///     lifetime scope.
    /// </summary>
    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InstancePerTestClass<TLimit, TActivatorData, TStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
    {
        if (registration == null) throw new ArgumentNullException(nameof(registration));

        return registration.InstancePerMatchingLifetimeScope(AutofacTestScopes.TestClass);
    }

    /// <summary>
    ///     Registers a component so all dependant components will resolve the same shared instance within the test collection
    ///     lifetime scope.
    /// </summary>
    public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InstancePerTestCollection<TLimit, TActivatorData, TStyle>(
        this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration)
    {
        if (registration == null) throw new ArgumentNullException(nameof(registration));

        return registration.InstancePerMatchingLifetimeScope(AutofacTestScopes.TestCollection);
    }
}
