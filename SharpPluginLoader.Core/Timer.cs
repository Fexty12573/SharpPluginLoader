using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core;

/// <summary>
/// Represents an instance of the cTimer class
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 0x18)]
public struct Timer
{
    [FieldOffset(0x00)] public nint VTable;
    [FieldOffset(0x08)] public float Time;
    [FieldOffset(0x0C)] public float MaxTime;
    [FieldOffset(0x10)] public bool Active;
    [FieldOffset(0x11)] public bool LockAtMax;

    /// <summary>
    /// Adds time to the timer. If the timer reaches the maximum time, it will return true.
    /// Should be called every frame.
    /// </summary>
    /// <param name="t">The time to add</param>
    /// <returns>True if the timer reached the maximum time, false otherwise</returns>
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

    /// <summary>
    /// Resets the timer to 0 and deactivates it.
    /// </summary>
    public void Reset()
    {
        Time = 0;
        Active = false;
    }

    /// <summary>
    /// Returns true if the timer has reached the maximum time.
    /// </summary>
    /// <returns>True if the timer has reached the maximum time, false otherwise</returns>
    public readonly bool Ended()
    {
        return Time >= MaxTime;
    }

    /// <summary>
    /// Sets the timer to the maximum time.
    /// </summary>
    public void SetToEnd()
    {
        Time = MaxTime;
    }
}

