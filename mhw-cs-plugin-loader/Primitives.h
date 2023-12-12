#pragma once

#include <dti/dti_types.h>

namespace primitives {

struct Sphere {
    MtSphere sphere;
    MtColorF color;
    Sphere(const MtSphere& sphere, MtColorF color) : sphere(sphere), color(color) {}
};

struct Capsule {
    MtCapsule capsule;
    MtColorF color;
    Capsule(const MtCapsule& capsule, MtColorF color) : capsule(capsule), color(color) {}
};

struct OBB {
    MtOBB obb;
    MtColorF color;
    OBB(const MtOBB& obb, MtColorF color) : obb(obb), color(color) {}
};

}
