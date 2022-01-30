using System;
using System.Reflection;

namespace Xunit.Frameworks.Autofac.TestFramework;

internal static class ExceptionExtensions
{
    /// <summary>
    ///     Unwraps an exception to remove any wrappers, like <see cref="TargetInvocationException" />.
    /// </summary>
    /// <param name="ex">The exception to unwrap.</param>
    /// <returns>The unwrapped exception.</returns>
    public static Exception Unwrap(this Exception ex)
    {
        while (true)
        {
            if (ex is not TargetInvocationException targetInvocationException)
            {
                return ex;
            }

            ex = targetInvocationException.InnerException;
        }
    }
}
