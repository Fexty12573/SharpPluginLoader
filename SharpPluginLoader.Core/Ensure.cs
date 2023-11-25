using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Provides methods to ensure that arguments are not null or default
    /// </summary>
    public static class Ensure
    {
        public static void NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string name = "") where T : class
        {
            if (value == null)
            {
                Log.Error($"'{name}' cannot be null");
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullOrEmpty(string value, [CallerArgumentExpression(nameof(value))] string name = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                Log.Error($"'{name}' cannot be null or empty");
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullOrDefault<T>([DisallowNull] T? value,
            [CallerArgumentExpression(nameof(value))] string name = "") where T : struct
        {
            if (value == null || value.Equals(default(T)))
            {
                Log.Error($"'{name}' cannot be null or default");
                throw new ArgumentNullException(name);
            }
        }

        public static void IsTrue(bool value, [CallerArgumentExpression(nameof(value))] string name = "")
        {
            if (!value)
            {
                Log.Error($"'{name}' must be true");
                throw new ArgumentException($"'{name}' must be true");
            }
        }
    }
}
