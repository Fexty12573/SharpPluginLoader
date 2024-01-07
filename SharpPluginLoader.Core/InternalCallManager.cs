using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal readonly struct InternalCall
    {
        private readonly nint _fieldName;
        public readonly nint FunctionPointer;

        public string Name => Marshal.PtrToStringAnsi(_fieldName) ?? string.Empty;
    }

    /// <summary>
    /// This attribute is used to mark a class as an internal call manager.
    /// There can only be one internal call manager per plugin.
    ///
    /// The internal call manager is where the plugin keeps all of its internal calls.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InternalCallManagerAttribute : Attribute;

    internal static class InternalCallManager
    {
        public static unsafe void UploadInternalCalls(InternalCall* internalCalls, uint internalCallsCount)
        {
            for (var i = 0; i < internalCallsCount; i++)
            {
                var icall = internalCalls[i];

                Log.Debug($"[Core] Uploading internal call {icall.Name}");

                var field = typeof(InternalCalls).GetField(icall.Name + "Ptr");

                if (field == null)
                {
                    Log.Error($"[Core] Internal call {icall.Name} does not exist");
                    continue;
                }

                if (!field.FieldType.IsFunctionPointer)
                {
                    Log.Error($"[Core] Internal call {icall.Name} is not a function pointer");
                    continue;
                }

                field.SetValue(null, icall.FunctionPointer);
            }
        }
    }
}
