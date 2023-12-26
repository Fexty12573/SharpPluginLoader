using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core;


[StructLayout(LayoutKind.Explicit, Size = 0x18)]
public struct Timer
{
    [FieldOffset(0x00)] public nint VTable;
    [FieldOffset(0x08)] public float Time;
    [FieldOffset(0x0C)] public float MaxTime;
    [FieldOffset(0x10)] public bool Active;
    [FieldOffset(0x11)] public bool LockAtMax;

    public bool AddTime(float t)
    {
        if (!Active)
            return false;

        Time += t;
        if (Time >= MaxTime)
        {
            if (LockAtMax)
                Time = MaxTime;
            else
                Active = false;
            return true;
        }

        return false;
    }

    public void Reset()
    {
        Time = 0;
        Active = false;
    }

    public readonly bool Ended()
    {
        return Time >= MaxTime;
    }

    public void SetToEnd()
    {
        Time = MaxTime;
    }
}

