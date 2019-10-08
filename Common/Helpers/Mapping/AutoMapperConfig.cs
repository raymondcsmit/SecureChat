using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace Helpers.Mapping
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig():
            this(AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => !asm.IsDynamic && asm.CodeBase.Contains("SecureChat")))
        {
        }

        public AutoMapperConfig(IEnumerable<Assembly> assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetTypes()).ToList();

            RegisterStandardFromMappings(types);
            RegisterStandardToMappings(types);
            RegisterCustomMaps(types);
        }

        private void RegisterStandardFromMappings(IEnumerable<Type> types) 
            => CreateMappings(GetFromMaps(types));

        private void RegisterStandardToMappings(IEnumerable<Type> types) 
            => CreateMappings(GetToMaps(types));

        private void RegisterCustomMaps(IEnumerable<Type> types) 
            => CreateMappings(GetCustomMappings(types));

        private static IEnumerable<IHaveCustomMappings> GetCustomMappings(IEnumerable<Type> types)
        {
            var customMaps = from t in types
                from i in t.GetTypeInfo().GetInterfaces()
                where typeof(IHaveCustomMappings).GetTypeInfo().IsAssignableFrom(t) &&
                      !t.GetTypeInfo().IsAbstract &&
                      !t.GetTypeInfo().IsInterface
                select (IHaveCustomMappings)Activator.CreateInstance(t);

            return customMaps;
        }

        private static IEnumerable<TypeMap> GetFromMaps(IEnumerable<Type> types)
        {
            var fromMaps = from t in types
                           from i in t.GetTypeInfo().GetInterfaces()
                           where i.GetTypeInfo().IsGenericType &&
                                 i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                                 !t.GetTypeInfo().IsAbstract &&
                                 !t.GetTypeInfo().IsInterface
                           select new TypeMap
                           {
                               Source = i.GetTypeInfo().GetGenericArguments().First(),
                               Destination = t
                           };

            return fromMaps;
        }

        private static IEnumerable<TypeMap> GetToMaps(IEnumerable<Type> types)
        {
            var toMaps = from t in types
                         from i in t.GetTypeInfo().GetInterfaces()
                         where i.GetTypeInfo().IsGenericType &&
                               i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                               !t.GetTypeInfo().IsAbstract &&
                               !t.GetTypeInfo().IsInterface
                         select new TypeMap
                         {
                             Source = t,
                             Destination = i.GetTypeInfo().GetGenericArguments().First()
                         };

            return toMaps;
        }

        private void CreateMappings(IEnumerable<TypeMap> maps)
        {
            foreach (var map in maps)
            {
                CreateMap(map.Source, map.Destination);
            }
        }

        private void CreateMappings(IEnumerable<IHaveCustomMappings> maps)
        {
            foreach (var map in maps)
            {
                map.CreateMappings(this);
            }
        }
    }
}
