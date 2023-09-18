using System;
using System.Reflection;

namespace ReceiptSharing.Api.Helpers
{
    public static class ObjectPropertyLogger
    {
        public static void LogProperties(object obj)
        {
            if (obj == null)
            {
                Console.WriteLine("Object is null.");
                return;
            }

            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            Console.WriteLine($"Properties of {type.Name}:");

            foreach (var property in properties)
            {
                object value = property.GetValue(obj)!;
                Console.WriteLine($"{property.Name}: {value}");
            }
        }
    }
}






