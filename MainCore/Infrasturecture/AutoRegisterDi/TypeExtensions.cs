using MainCore.Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MainCore.Infrasturecture.AutoRegisterDi
{
    /// <summary>
    /// Extensions for <see cref="Type"/>
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Check if type marked by <see cref="DoNotAutoRegisterAttribute"/>
        /// </summary>
        /// <param name="type">type</param>
        public static bool IsIgnoredType(this Type type)
        {
            var doNotAutoRegisterAttribute = type.GetCustomAttribute<DoNotAutoRegisterAttribute>();
            return doNotAutoRegisterAttribute != null;
        }

        /// <summary>
        /// Check if class marked by attributes
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static bool HasAttribute(this Type type)
        {
            return type.GetCustomAttributes(typeof(RegisterWithLifetimeAttribute), true).Length == 1;
        }

        /// <summary>
        /// Returns service lifetime
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="lifetime">If no attribute then it returns the lifetime provided in the AsPublicImplementedInterfaces parameter</param>
        /// <returns></returns>
        public static ServiceLifetime GetLifetimeForClass(this Type type, ServiceLifetime lifetime)
        {
            return type.GetCustomAttribute<RegisterWithLifetimeAttribute>(true)?.RequiredLifetime ?? lifetime;
        }

        /// <summary>
        /// Returns service server
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="server">If no attribute then it returns the server provided in the AsPublicImplementedInterfaces parameter</param>
        /// <returns></returns>
        public static ServerEnums GetServerForClass(this Type type)
        {
            return type.GetCustomAttribute<RegisterWithLifetimeAttribute>(true)?.RequiredServer ?? ServerEnums.NONE;
        }

        public static bool WithoutInterface(this Type type)
        {
            return type.GetCustomAttribute<RegisterWithLifetimeAttribute>(true)?.WithoutInterface ?? false;
        }
    }
}