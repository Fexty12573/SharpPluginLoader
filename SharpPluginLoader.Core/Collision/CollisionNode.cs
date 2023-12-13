using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Collision;

/// <summary>
/// Represents a cCollisionNode object in the game.
/// </summary>
public class CollisionNode : MtObject
{
    public CollisionNode(nint instance) : base(instance) { }
    public CollisionNode() { }

    /// <summary>
    /// Whether the node is active.
    /// </summary>
    public ref bool IsActive => ref GetRef<bool>(0x8);

    /// <summary>
    /// The geometry array of the node.
    /// </summary>
    public MtArray<MtObject> GeometryArray => GetInlineObject<MtArray<MtObject>>(0x18);

    /// <summary>
    /// The bounding box of the node.
    /// </summary>
    public ref MtAabb BoundingBox => ref GetRef<MtAabb>(0x28);
}
