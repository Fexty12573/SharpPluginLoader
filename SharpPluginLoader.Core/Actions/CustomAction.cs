using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Actions;


public delegate void OnActionInitialize(nint action);
public delegate void OnActionExecute(nint action);
[return: MarshalAs(UnmanagedType.I1)] public delegate bool OnActionUpdate(nint action);
public delegate void OnActionEnd(nint action);

public delegate void OnInitializeCallback(Action action, OnActionInitialize parentFunc, OnActionInitialize baseFunc);
public delegate void OnExecuteCallback(Action action, OnActionExecute parentFunc, OnActionExecute baseFunc);
public delegate bool OnUpdateCallback(Action action, OnActionUpdate parentFunc, OnActionUpdate baseFunc);
public delegate void OnEndCallback(Action action, OnActionEnd parentFunc, OnActionEnd baseFunc);

public class CustomAction
{
    /// <summary>
    /// The name of the action. Has no effect on functionality. Can be null.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The flags of the action. -1 for default.
    /// </summary>
    public int Flags { get; init; }

    /// <summary>
    /// The Id of the action to use as a base for the custom action. If you set this
    /// leave BaseDti as nullptr
    /// </summary>
    public int BaseActionId { get; init; }

    /// <summary>
    /// The DTI of the action to use as a base for the custom action.
    /// Use this if the action you want to use as a base is from a different monster.
    /// If you set this BaseActionId will be ignored
    /// </summary>
    public MtDti? BaseDti { get; init; }

    /// <summary>
    /// The OnInitialize function will be called after the action object is created
    /// </summary>
    public OnInitializeCallback? OnInitialize { get; init; }

    /// <summary>
    /// The OnExecute function will be called each time the action is executed
    /// </summary>
    public OnExecuteCallback? OnExecute { get; init; }

    /// <summary>
    /// The OnUpdate function will be called once per frame while the action is active. Return false to end the action
    /// </summary>
    public OnUpdateCallback? OnUpdate { get; init; }

    /// <summary>
    /// The OnEnd function will be called when the action ends
    /// </summary>
    public OnEndCallback? OnEnd { get; init; }

    /// <summary>
    /// A dictionary of virtual functions to override. The key is the index of the virtual function
    /// </summary>
    public Dictionary<int, Delegate> VirtualFunctions { get; } = [];

    public CustomAction(string? name,
        int baseActionId,
        OnInitializeCallback? onInitialize = null,
        OnExecuteCallback? onExecute = null,
        OnUpdateCallback? onUpdate = null,
        OnEndCallback? onEnd = null,
        int flags = -1)
    {
        Name = name;
        Flags = flags;
        BaseActionId = baseActionId;
        BaseDti = null;
        OnInitialize = onInitialize;
        OnExecute = onExecute;
        OnUpdate = onUpdate;
        OnEnd = onEnd;
    }

    public CustomAction(string? name,
        MtDti baseDti,
        OnInitializeCallback? onInitialize = null,
        OnExecuteCallback? onExecute = null,
        OnUpdateCallback? onUpdate = null,
        OnEndCallback? onEnd = null,
        int flags = -1)
    {
        Name = name;
        Flags = flags;
        BaseActionId = -1;
        BaseDti = baseDti;
        OnInitialize = onInitialize;
        OnExecute = onExecute;
        OnUpdate = onUpdate;
        OnEnd = onEnd;
    }

    public CustomAction() { }

    ~CustomAction()
    {
        if (VTable.Address != 0)
        {
            VTable.Dispose();
        }
    }

    internal NativeArray<nint> VTable { get; set; }

    internal OnActionInitialize? ParentOnInitialize { get; set; }
    internal OnActionExecute? ParentOnExecute { get; set; }
    internal OnActionUpdate? ParentOnUpdate { get; set; }
    internal OnActionEnd? ParentOnEnd { get; set; }

    internal OnActionInitialize? OnInitializeWrapper { get; set; }
    internal OnActionExecute? OnExecuteWrapper { get; set; }
    internal OnActionUpdate? OnUpdateWrapper { get; set; }
    internal OnActionEnd? OnEndWrapper { get; set; }
}
