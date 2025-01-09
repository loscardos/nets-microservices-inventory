using System.Globalization;
using System.Reflection;
using System.Text;

namespace InventoryService.Infrastructure.Helpers
{
    public static class CustomExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> dictionary) where T : new()
        {
            T obj = new T();
            Type type = typeof(T);

            foreach (var kvp in dictionary)
            {
                try
                {
                    // Convert snake_case to PascalCase
                    string propertyName = SnakeCaseToPascalCase(kvp.Key);

                    PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (property == null)
                    {
                        Console.WriteLine($"Warning: Property '{propertyName}' not found on {typeof(T).Name}. Skipping...");
                        continue;
                    }

                    if (!property.CanWrite)
                    {
                        Console.WriteLine($"Warning: Property '{propertyName}' is read-only. Skipping...");
                        continue;
                    }

                    if (kvp.Value == null)
                    {
                        Console.WriteLine($"Warning: Value for property '{propertyName}' is null. Skipping...");
                        continue;
                    }

                    object value = Convert.ChangeType(kvp.Value, property.PropertyType);
                    property.SetValue(obj, value);
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine($"Error: Unable to convert value of key '{kvp.Key}' to type {type.GetProperty(kvp.Key)?.PropertyType}. Skipping...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: An unexpected error occurred while setting property '{kvp.Key}'. Details: {ex.Message}");
                }
            }

            return obj;
        }
        
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetType()
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(prop => prop.CanRead)
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(obj, null) ?? DBNull.Value // Handle null values
                );
        }
        
        private static string SnakeCaseToPascalCase(string snakeCase)
        {
            var words = snakeCase.Split('_');
            var sb = new StringBuilder();
            foreach (var word in words)
            {
                sb.Append(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word.ToLower()));
            }
            return sb.ToString();
        }
    }
}