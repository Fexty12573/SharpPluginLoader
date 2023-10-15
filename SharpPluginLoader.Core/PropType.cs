﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public enum PropType
    {
        Undefined,
        Class,
        ClassRef,
        Bool,
        U8,
        U16,
        U32,
        U64,
        S8,
        S16,
        S32,
        S64,
        F32,
        F64,
        String,
        Color,
        Point,
        Size,
        Rect,
        Matrix44,
        Vector3,
        Vector4,
        Quaternion,
        Property,
        Event,
        Group,
        PageBegin,
        PageEnd,
        Event32,
        Array,
        PropertyList,
        GroupEnd,
        CString,
        Time,
        Float2,
        Float3,
        Float4,
        Float3X3,
        Float4X3,
        Float4X4,
        Easecurve,
        Line,
        Linesegment,
        Ray,
        Plane,
        Sphere,
        Capsule,
        Aabb,
        Obb,
        Cylinder,
        Triangle,
        Cone,
        Torus,
        Ellipsoid,
        Range,
        RangeF,
        RangeU16,
        Hermitecurve,
        Enumlist,
        Float3X4,
        LineSegment4,
        Aabb4,
        Oscillator,
        Variable,
        Vector2,
        Matrix33,
        Rect3dXz,
        Rect3d,
        Rect3dCollision,
        PlaneXz,
        RayY,
        PointF,
        SizeF,
        RectF,
        Event64,
        Bool2,
        End
    }
}