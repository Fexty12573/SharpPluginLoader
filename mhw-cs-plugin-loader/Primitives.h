#pragma once

#include <dti/dti_types.h>

namespace primitives {

dti_size_assert(MtSphere, 0x10);
struct Sphere {
    MtSphere sphere;
    MtColorF color;
    Sphere(const MtSphere& sphere, MtColorF color) : sphere(sphere), color(color) {}
};

dti_offset_assert(MtCapsule, p1, 0x10);
dti_offset_assert(MtCapsule, r, 0x20);
struct Capsule {
    MtCapsule capsule;
    MtColorF color;
    Capsule(const MtCapsule& capsule, MtColorF color) : capsule(capsule), color(color) {}
};

dti_offset_assert(MtOBB, extent, 0x40);
dti_size_assert(MtOBB, 0x50);
struct OBB {
    MtOBB obb;
    MtColorF color;
    OBB(const MtOBB& obb, MtColorF color) : obb(obb), color(color) {}
};

}
