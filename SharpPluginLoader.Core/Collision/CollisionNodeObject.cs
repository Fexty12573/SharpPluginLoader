using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Collision;

/// <summary>
/// Represents a cCollisionNodeObject object in the game.
/// </summary>
public class CollisionNodeObject : CollisionNode
{
    public CollisionNodeObject(nint instance) : base(instance) { }
    public CollisionNodeObject() { }

    /// <summary>
    /// The object that owns this node.
    /// </summary>
    public MtObject? Owner => GetObject<MtObject>(0x80);

    /// <summary>
    /// The attributes of the node.
    /// </summary>
    public ref uint Attributes => ref GetRef<uint>(0x98);

    /// <summary>
    /// The move vector of the node.
    /// </summary>
    public ref Vector4 MoveVector => ref GetRef<Vector4>(0xA0);
}
