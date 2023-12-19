using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Resources.Collision;

namespace SharpPluginLoader.Core.Resources;

/// <summary>
/// Represents an instance of a rObjCollision resource.
/// </summary>
public class ObjCollision : Resource
{
    public ObjCollision(nint instance) : base(instance) { }
    public ObjCollision() { }

    /// <summary>
    /// Gets the associated <see cref="CollIndexResource"/> object.
    /// </summary>
    public CollIndexResource? CollIndex => GetObject<CollIndexResource>(0xA8);

    /// <summary>
    /// Gets the associated <see cref="CollNodeResource"/> object.
    /// </summary>
    public CollNodeResource? CollNode => GetObject<CollNodeResource>(0xB0);

    /// <summary>
    /// Gets the associated <see cref="AttackParamResource"/> object.
    /// </summary>
    public AttackParamResource? AttackParam => GetObject<AttackParamResource>(0xB8);

    /// <summary>
    /// Gets the associated rObjAppendParam object.
    /// </summary>
    public Resource? ObjAppendParam => GetObject<Resource>(0xC0);
}
