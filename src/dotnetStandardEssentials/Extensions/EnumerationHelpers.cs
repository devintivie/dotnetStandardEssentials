using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetStandardEssentials.Extensions
{
    public static class EnumerationHelpers
    {
        public static IEnumerable<T> GetPublicDeclaredStaticInstances<T>()
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
        }
    }
}
