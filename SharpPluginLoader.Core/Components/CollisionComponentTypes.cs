using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Collision;
using SharpPluginLoader.Core.Geometry;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Components;

public partial class CollisionComponent
{
    public partial class Node
    {
        public Node(nint instance) : base(instance) { }
        public Node() { }

        /// <summary>
        /// Gets the world matrix of this node.
        /// </summary>
        public ref MtMatrix4X4 WorldMatrix => ref GetRef<MtMatrix4X4>(0x10);

        /// <summary>
        /// Gets the geometry of this node.
        /// </summary>
        public MtArray<Geometry> Geometries => GetInlineObject<MtArray<Geometry>>(0x50);

        /// <summary>
        /// Gets the unique id of this node.
        /// </summary>
        public ref uint UniqueId => ref GetRef<uint>(0x70);

        /// <summary>
        /// Gets the owning <see cref="Components.CollisionComponent"/> of this node.
        /// </summary>
        public CollisionComponent? CollisionComponent => GetObject<CollisionComponent>(0x78);

        /// <summary>
        /// Gets the collision node of this node.
        /// </summary>
        public CollisionNodeObject? CollisionNode => GetObject<CollisionNodeObject>(0x80);

        /// <summary>
        /// Whether this node is active.
        /// </summary>
        public ref bool IsActive => ref GetRef<bool>(0xB1);
    }

    public partial class Geometry
    {
        public Geometry(nint instance) : base(instance) { }
        public Geometry() { }

        public MtGeometry? Geom => GetObject<MtGeometry>(0x18);
    }
}
