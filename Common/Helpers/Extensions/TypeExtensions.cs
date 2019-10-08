using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsGenericInterface(this Type type, Type interfaceToCheck, bool includeInherited = true)
            => type.GetInterfaces(includeInherited)
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceToCheck);

        public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
            => type.GetInterfaces()
                .Except(!includeInherited && type.BaseType != null ? type.BaseType?.GetInterfaces() : Enumerable.Empty<Type>());

        public static string GetGenericTypeName(this Type type)
        {
            var typeName = string.Empty;

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static string GetGenericTypeName(this object @object) => @object.GetType().GetGenericTypeName();
    }
}
