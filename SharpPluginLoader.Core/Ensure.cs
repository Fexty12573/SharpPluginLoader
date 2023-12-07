using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Provides methods to ensure that arguments are not null or default
    /// </summary>
    public static class Ensure
    {
        public static void NotNull<T>(
            [NotNull] T? value, 
            [CallerArgumentExpression(nameof(value))] string name = "") where T : class
        {
            if (value is null)
            {
                Log.Error($"'{name}' cannot be null");
                throw new ArgumentNullException(name);
            }
        }

        public static unsafe void NotNull<T>(
            [NotNull] T* value,
            [CallerArgumentExpression(nameof(value))]
            string name = "") where T : unmanaged
        {
            if (value is null)
            {
                Log.Error($"'{name}' cannot be null");
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullOrEmpty(
            [NotNull] string? value, 
            [CallerArgumentExpression(nameof(value))] string name = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                Log.Error($"'{name}' cannot be null or empty");
                throw new ArgumentNullException(name);
            }
        }

        public static void NotNullOrDefault<T>(
            [NotNull] T? value,
            [CallerArgumentExpression(nameof(value))] string name = "") where T : struct
        {
            if (value is null || value.Equals(default(T)))
            {
                Log.Error($"'{name}' cannot be null or default");
                throw new ArgumentNullException(name);
            }
        }

        public static void IsTrue(
            [DoesNotReturnIf(false)] bool value, 
            [CallerArgumentExpression(nameof(value))] string name = "")
        {
            if (!value)
            {
                Log.Error($"'{name}' must be true");
                throw new ArgumentException($"'{name}' must be true");
            }
        }
    }
}
