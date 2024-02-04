using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.Resources;

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
    /// Gets the <see cref="ObjCollision"/> resources of this <see cref="CollisionComponent"/>.
    /// </summary>
    public NativeArray<nint> Resources => new(Instance + 0xD0, 8);

    /// <inheritdoc cref="Resources"/>
    public ObjCollision[] Collisions => Resources.Where(x => x != 0).Select(x => new ObjCollision(x)).ToArray();

    /// <summary>
    /// Gets the owner of this <see cref="CollisionComponent"/>.
    /// </summary>
    public Model? Model => GetObject<Model>(0x1A8);

    public partial class Node : MtObject;
    public partial class Geometry : MtObject;
}
