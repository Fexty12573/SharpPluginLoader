using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Dti;

/// <summary>
/// Performs cleanup operations on a custom <see cref="MtDti"/> instance
/// </summary>
/// <param name="dti">The <see langword="this"/> pointer</param>
public delegate void DtiDtorDelegate(MtDti dti);

/// <summary>
/// Creates a new instance of the type represented by the <see cref="MtDti"/> instance
/// </summary>
/// <param name="dti">The <see langword="this"/> pointer</param>
/// <returns>The new instance, or <see langword="null"/> if the operation failed</returns>
public delegate MtObject? DtiNewDelegate(MtDti dti);

/// <summary>
/// Initializes a new instance of the type represented by the <see cref="MtDti"/> instance.
/// This essentially acts as a constructor for the type.
/// </summary>
/// <param name="dti">The <see langword="this"/> pointer</param>
/// <param name="obj">The object to initialize</param>
/// <returns>The initialized object, or <see langword="null"/> if the operation failed</returns>
public delegate MtObject? DtiCtorDelegate(MtDti dti, MtObject? obj);

/// <summary>
/// Initializes an array of objects of the type represented by the <see cref="MtDti"/> instance.
/// </summary>
/// <param name="dti">The <see langword="this"/> pointer</param>
/// <param name="obj">The array of objects to initialize</param>
/// <returns>The initialized array of objects</returns>
public delegate MtObject?[] DtiCtorArrayDelegate(MtDti dti, MtObject?[] obj);


/// <inheritdoc cref="DtiDtorDelegate"/>
internal delegate void NativeDtiDtorDelegate(nint dti);

/// <inheritdoc cref="DtiNewDelegate"/>
internal delegate nint NativeDtiNewDelegate(nint dti);

/// <inheritdoc cref="DtiCtorDelegate"/>
internal delegate nint NativeDtiCtorDelegate(nint dti, nint obj);

/// <inheritdoc cref="DtiCtorArrayDelegate"/>
internal delegate nint NativeDtiCtorArrayDelegate(nint dti, nint objects, int count);
