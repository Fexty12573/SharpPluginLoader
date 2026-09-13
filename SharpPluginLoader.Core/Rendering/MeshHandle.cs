namespace SharpPluginLoader.Core.Rendering;

/// <summary>
/// Represents a mesh handle that can be passed to the <see cref="Primitives"/> Renderer
/// </summary>
/// <param name="handle">The handle</param>
/// <remarks>
/// You should not create instances of this struct yourself. Handles can be retrieved via
/// <see cref="Primitives.SupplyCustomMesh"/>.
/// </remarks>
public readonly struct MeshHandle(nuint handle) : IEquatable<MeshHandle>
{
    private readonly nuint _handle = handle;

    public static implicit operator MeshHandle(nuint handle) => new(handle);
    public static implicit operator nuint(MeshHandle handle) => handle._handle;

    /// <summary>
    /// An invalid mesh handle.
    /// </summary>
    public static MeshHandle Invalid => new(nuint.Zero);

    #region Equality Operators

    public override bool Equals(object? obj) => obj is MeshHandle h && Equals(h);
    public bool Equals(MeshHandle other) => _handle == other._handle;

    public static bool operator ==(MeshHandle left, MeshHandle right) => left._handle == right._handle;
    public static bool operator !=(MeshHandle left, MeshHandle right) => left._handle != right._handle;
    public static bool operator ==(MeshHandle left, nuint right) => left._handle == right;
    public static bool operator !=(MeshHandle left, nuint right) => left._handle != right;
    public static bool operator ==(nuint left, MeshHandle right) => left == right._handle;
    public static bool operator !=(nuint left, MeshHandle right) => left != right._handle;

    #endregion

    public override int GetHashCode() => _handle.GetHashCode();
}
