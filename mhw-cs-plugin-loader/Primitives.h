#pragma once

#include <dti/dti_types.h>

namespace primitives {

dti_size_assert(MtSphere, 0x10);
struct Sphere {
    MtSphere sphere;
    MtColorF color;
    Sphere(const MtSphere& sphere, const MtColorF& color) : sphere(sphere), color(color) {}
};

dti_offset_assert(MtCapsule, p1, 0x10);
dti_offset_assert(MtCapsule, r, 0x20);
struct Capsule {
    MtCapsule capsule;
    MtColorF color;
    Capsule(const MtCapsule& capsule, const MtColorF& color) : capsule(capsule), color(color) {}
};

dti_offset_assert(MtOBB, extent, 0x40);
dti_size_assert(MtOBB, 0x50);
struct OBB {
    MtOBB obb;
    MtColorF color;
    OBB(const MtOBB& obb, const MtColorF& color) : obb(obb), color(color) {}
};

dti_offset_assert(MtLineSegment, p1, 0x10);
dti_size_assert(MtLineSegment, 0x20);
struct Line {
    MtLineSegment line;
    MtColorF color;
    Line(const MtLineSegment& line, const MtColorF& color) : line(line), color(color) {}
};

}
