using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Xunit.Abstractions;

namespace Xunit.Frameworks.Autofac.TestFramework;

internal static class AutofacTestExtensions
{
    public static void RegisterCollectionFixturesAndModules(this ContainerBuilder builder, ITestCollection testCollection)
    {
        if (testCollection.CollectionDefinition == null) return;

        Type declarationType = ((IReflectionTypeInfo)testCollection.CollectionDefinition).Type;
        builder.RegisterModules(declarationType);
        foreach (Type fixtureType in declarationType.GetTypeParametersFromInterfaces(typeof(ICollectionFixture<>)))
        {
            builder.RegisterModules(fixtureType);
            builder.RegisterType(fixtureType).AsSelf().SingleInstance();
        }
    }

    public static void RegisterClassFixturesAndModules(this ContainerBuilder builder, ITestClass testClass, IReflectionTypeInfo @class)
    {
        foreach (Type fixtureType in @class.Type.GetTypeParametersFromInterfaces(typeof(IClassFixture<>)))
        {
            builder.RegisterModules(fixtureType);
            builder.RegisterType(fixtureType).AsSelf().SingleInstance();
        }

        if (testClass.TestCollection.CollectionDefinition != null)
        {
            Type declarationType = ((IReflectionTypeInfo)testClass.TestCollection.CollectionDefinition).Type;
            foreach (Type fixtureType in declarationType.GetTypeParametersFromInterfaces(typeof(IClassFixture<>)))
            {
                builder.RegisterModules(fixtureType);
                builder.RegisterType(fixtureType).AsSelf().SingleInstance();
            }
        }
    }

    public static void RegisterTheoryFixturesAndModules(this ContainerBuilder builder, ITestClass testClass, IReflectionTypeInfo @class)
    {
        foreach (Type fixtureType in @class.Type.GetTypeParametersFromInterfaces(typeof(ITheoryFixture<>)))
        {
            builder.RegisterModules(fixtureType);
            builder.RegisterType(fixtureType).AsSelf().SingleInstance();
        }

        if (testClass.TestCollection.CollectionDefinition != null)
        {
            Type declarationType = ((IReflectionTypeInfo)testClass.TestCollection.CollectionDefinition).Type;
            foreach (Type fixtureType in declarationType.GetTypeParametersFromInterfaces(typeof(ITheoryFixture<>)))
            {
                builder.RegisterModules(fixtureType);
                builder.RegisterType(fixtureType).AsSelf().SingleInstance();
            }
        }
    }

    public static void RegisterModules(this ContainerBuilder builder, Type type)
    {
        foreach (Type moduleType in type.GetTypeParametersFromInterfaces(typeof(INeedModule<>)))
        {
            builder.RegisterModule((IModule)Activator.CreateInstance(moduleType));
        }
    }

    public static IEnumerable<Type> GetTypeParametersFromInterfaces(this Type declarationType, Type genericInterfaceType)
    {
        return declarationType.GetTypeInfo()
                              .ImplementedInterfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType)
                              .Select(interfaceType => interfaceType.GenericTypeArguments.Single());
    }
}
