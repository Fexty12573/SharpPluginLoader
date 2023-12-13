using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Models;

namespace SharpPluginLoader.Core.Components;

public partial class CollisionComponent : Component
{
    public CollisionComponent(nint instance) : base(instance) { }
    public CollisionComponent() { }

    /// <summary>
    /// Gets the <see cref="Node"/>s of this <see cref="CollisionComponent"/>.
    /// </summary>
    public MtArray<Node> Nodes => GetInlineObject<MtArray<Node>>(0x150);

    /// <summary>
    /// Gets the owner of this <see cref="CollisionComponent"/>.
    /// </summary>
    public Model? Model => GetObject<Model>(0x1A8);

    public partial class Node : MtObject;
    public partial class Geometry : MtObject;
}
