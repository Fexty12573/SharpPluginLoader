namespace SharpPluginLoader.Core
{
    internal static class InternalCalls
    {
        public static unsafe delegate* unmanaged<void> TestInternalCallPtr;
        public static unsafe delegate* unmanaged<nint, void> QueueYesNoDialogPtr;

        public static unsafe void TestInternalCall() => TestInternalCallPtr();

        public static unsafe void QueueYesNoDialog(nint messagePtr) => QueueYesNoDialogPtr(messagePtr);
    }
}
