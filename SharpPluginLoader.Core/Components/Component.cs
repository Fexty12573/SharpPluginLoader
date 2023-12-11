using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Components;

public class Component : MtObject
{
    public Component(nint instance) : base(instance) { }
    public Component() { }

    /// <summary>
    /// Gets the next component in the list.
    /// </summary>
    public Component? Next => GetObject<Component>(0x8);
}
