using DotNetVersionInfo;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tests
{
    static class ReflectionHelper
    {
        public static void SetPropertyBackingField(
            Type type,
            object instance,
            string propertyName,
            object value)
        {
            var backingFieldName = $"_{propertyName.ToCamelCase()}";
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var backingField = fields.First(field => field.Name == backingFieldName);
            backingField.SetValue(instance, value);
        }

        private static string ToCamelCase(this string s) =>
            s.ToLowerInvariant()[0] + s.Substring(1);
    }
}
