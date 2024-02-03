#pragma once

#include <string>

#define dti_size_assert(T, size) static_assert(sizeof(T) == size, "Size of " #T " is not " #size " bytes")
#define dti_offset_assert(T, member, offset) static_assert(offsetof(T, member) == offset, "Offset of " #T "::" #member " is not at offset " #offset)

#if defined(min) || defined(max)
#undef min
#undef max
#define REDEFINE_MINMAX
#endif

typedef unsigned char u8;
typedef unsigned short u16;
typedef unsigned int u32;
typedef unsigned long long u64;
typedef signed char s8;
typedef signed short s16;
typedef signed int s32;
typedef signed long long s64;
typedef float f32;
typedef double f64;

typedef u32 color; // 0xRRGGBBAA
typedef f32 matrix44[4][4]; // [R][C]
typedef f32 float2[2];
typedef f32 float3[3];
typedef f32 float4[4];
typedef char* cstring;
typedef char* string;
typedef f32 float3x3[3][3];
typedef f32 float4x3[4][3];
typedef f32 float3x4[3][4];
typedef f32 float4x4[4][4];
typedef f32 matrix33[3][3]; // [R][C]
typedef void* classref;
typedef u64 time_;

struct MtObject;
typedef void(*MT_MFUNC)(MtObject* pthis);
typedef void(*MT_MFUNC32)(MtObject* pthis, u32 param);
typedef void(*MT_MFUNC64)(MtObject* pthis, u64 param);
typedef void(*MT_MFUNC32X2)(MtObject* pthis, u32 param1, u32 param2);
typedef void(*MT_MFUNC64X2)(MtObject* pthis, u64 param1, u64 param2);
typedef void(*MT_MFUNCPTR)(MtObject* pthis, void* param);
typedef void(*MT_MFUNCPTRX2)(MtObject* pthis, void* param1, void* param2);
typedef void(*MT_MFUNCPTRU32)(MtObject* pthis, void* param1, u32 param2);

struct point
{
    u32 x, y;
};
struct MtSize
{
    u32 x, y;
};
struct MtRect
{
    u32 x, y, w, h;
};
struct vector3
{
    f32 x, y, z;
    vector3& operator+=(const vector3& v) {
        x += v.x;
        y += v.y;
        z += v.z;
        return *this;
    }
    vector3& operator-=(const vector3& v) {
        x -= v.x;
        y -= v.y;
        z -= v.z;
        return *this;
    }
    vector3& operator*=(float v) {
        x *= v;
        y *= v;
        z *= v;
        return *this;
    }
    vector3& operator/=(float v) {
        x /= v;
        y /= v;
        z /= v;
        return *this;
    }
    vector3& operator+=(float v) {
        x += v;
        y += v;
        z += v;
        return *this;
    }
    vector3& operator-=(float v) {
        x -= v;
        y -= v;
        z -= v;
        return *this;
    }

    vector3 operator+(const vector3& v) const { return vector3(*this) += v; }
    vector3 operator-(const vector3& v) const { return vector3(*this) -= v; }
    vector3 operator+(float v) const { return vector3(*this) += v; }
    vector3 operator-(float v) const { return vector3(*this) -= v; }
    vector3 operator*(float v) const { return vector3(*this) *= v; }
    vector3 operator/(float v) const { return vector3(*this) /= v; }
    vector3 operator-() const { return { -x, -y, -z }; }
    float operator*(const vector3& v) const { return dot(v); }

    bool operator==(const vector3& v) const { return x == v.x && y == v.y && z == v.z; }
    bool operator!=(const vector3& v) const { return !((*this) == v); }

    float lensq() const { return x * x + y * y + z * z; }
    float len() const { return sqrtf(lensq()); }

    float dot(const vector3& v) const { return x * v.x + y * v.y + z * v.z; }
    vector3 cross(const vector3& v) const { return { y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x }; }

    vector3& normalize() { return (*this) /= len(); }
    vector3 normalized() const { return vector3(*this) /= len(); }

    vector3& set_length(float length) { return normalize() *= length; }
    vector3& limit(float length) { return lensq() > (length * length) ? set_length(length) : *this; }
    float* data() { return &x; }
};
struct alignas(16) vector4
{
    f32 x, y, z, w;
};
struct alignas(16) quaternion
{
    union {
        f32 x, y, z, w;
        float4 values;
    };
};

struct easecurve
{
    union {
        float2 floats;
        f64 longfloat;
    };
};
struct line
{
    vector3 from, dir;
};
typedef line ray;

struct linesegment
{
    vector3 p0, p1;
};
typedef linesegment linesegment4[4];
struct Plane
{
    vector3 normal;
};
struct sphere
{
    float3 pos;
    f32 r;
};
struct capsule
{
    vector3 p0, p1;
    f32 r;
};
struct aabb
{
    alignas(16) vector3 minpos, maxpos;

	[[nodiscard]] vector3 center() const {
		return (minpos + maxpos) * 0.5f;
	}
};
typedef aabb aabb4[4];
struct obb
{
    matrix44 coord;
    vector3 extent;
};
struct cylinder
{
    vector3 p0;
    vector3 p1;
    f32 r;
};
struct triangle
{
    vector3 p0, p1, p2;
};
struct range
{
    s32 min, max;
};
struct rangef
{
    f32 min, max;
};
struct rangeu16
{
    u16 min, max;
};
struct hermitecurve
{
    float x[8];
    float y[8];

    hermitecurve(std::initializer_list<std::pair<float, float>> xy) {
        std::memset(this->x, 0, sizeof(this->x));
        std::memset(this->y, 0, sizeof(this->y));
        for (auto i = 0u; i < xy.size(); ++i) {
            this->x[i] = xy.begin()[i].first;
            this->y[i] = xy.begin()[i].second;
        }
    }

    hermitecurve() {
        std::memset(this->x, 0, sizeof(this->x));
        std::memset(this->y, 0, sizeof(this->y));
    }

    hermitecurve(const hermitecurve& other) {
        std::memcpy(this->x, other.x, sizeof(this->x));
        std::memcpy(this->y, other.y, sizeof(this->y));
    }

    hermitecurve& operator=(const hermitecurve& other) {
        std::memcpy(this->x, other.x, sizeof(this->x));
        std::memcpy(this->y, other.y, sizeof(this->y));
        return *this;
    }

    hermitecurve(hermitecurve&& other) noexcept {
        std::memcpy(this->x, other.x, sizeof(this->x));
        std::memcpy(this->y, other.y, sizeof(this->y));
    }

    hermitecurve& operator=(hermitecurve&& other) noexcept {
        std::memcpy(this->x, other.x, sizeof(this->x));
        std::memcpy(this->y, other.y, sizeof(this->y));
        return *this;
    }

    [[nodiscard]] static constexpr int point_count() { return 8; }
    [[nodiscard]] int effective_point_count() const {
        for (int i = 0; i < point_count(); ++i) {
            if (x[i] == 1.0f) {
                return i + 1;
            }
        }

        return point_count();
    }
    [[nodiscard]] float get(float xx) const {
        if (xx <= x[0]) {
            return y[0];
        }

        if (xx >= 1.0f) {
            for (int i = 0; i < 8; ++i) {
                if (x[i] == 1.0f) {
                    return y[i];
                }
            }
        }

        int n = 0;
        for (int i = 1; i < 8; ++i) {
            if (x[i] > xx) {
                n = i - 1;
                break;
            }
        }

        const float dx = x[n + 1] - x[n];
        float dy;

        if (n == 0) {
            dy = y[n + 1] - y[n];
        } else {
            const float dx0 = x[n] - x[n - 1];
            const float dy0 = y[n] - y[n - 1];
            dy = (y[n + 1] - ((1.0f - dx / dx0) * dy0 + y[n - 1])) * 0.5f;
        }

        float _dy;
        if (x[n + 1] == 1.0f || n > 5) {
            _dy = y[n + 1] - y[n];
        } else {
            const float dx1 = x[n + 2] - x[n + 1];
            const float dy1 = y[n + 2] - y[n + 1];
            _dy = ((dx / dx1) * dy1 + y[n + 1] - y[n]) * 0.5f;
        }

        const float ratio = (xx - x[n]) / dx;
        const float rsq = ratio * ratio;
        const float rcb = rsq * ratio;

        const float o2 = (2 * (y[n] - y[n + 1]) + dy + _dy) * rcb;
        const float o3 = (3 * (y[n + 1] - y[n]) - dy - dy - _dy) * rsq;

        const float result = o2 + o3 + ratio * dy + y[n];

        return std::max(std::min(result, 1.0f), 0.0f);
    }

    [[nodiscard]] float operator[](float xx) const {
        return get(xx);
    }
};
struct vector2
{
    f32 x, y;
};
typedef vector2 pointf;
typedef vector2 sizef;
struct rectf
{
    f32 x, y, w, h;
};
struct plane_xz
{
    vector3 normal;
};
struct pthread_mutex;
typedef pthread_mutex* pthread_mutex_t;


using MtVector2 = vector2;
using MtVector3 = vector3;
using MtVector4 = vector4;
using MtQuaternion = MtVector4;
using MtFloat3 = MtVector3;
using MtFloat4 = MtVector4;
using MtHermiteCurve = hermitecurve;

union MtColor
{
    u32 rgba;
    struct {
        u32 r : 8;
        u32 g : 8;
        u32 b : 8;
        u32 a : 8;
    };
};
struct MtMatrix
{
    MtVector4 m[4];
	[[nodiscard]] float* ptr() { return &m[0].x; }
	[[nodiscard]] const float* ptr() const { return &m[0].x; }
};
struct MtSphere
{
    MtFloat3 pos;
    f32 r;
};
struct MtAABB
{
    MtVector3 minpos;
    MtVector3 maxpos;
};
struct alignas(16) MtOBB
{
    MtMatrix coord;
    MtVector3 extent;
};
struct MtCapsule {
    alignas(16) MtVector3 p0;
    alignas(16) MtVector3 p1;
    alignas(16) f32 r;
};
struct MtColorF
{
    f32 r, g, b, a;
    MtColorF() = default;
    MtColorF(f32 r, f32 g, f32 b, f32 a) : r(r), g(g), b(b), a(a) {}
    MtColorF(const MtVector4& v) : r(v.x), g(v.y), b(v.z), a(v.w) {}
};
struct MtEaseCurve
{
    f32 p1, p2;
};
struct MtCriticalSection
{
    pthread_mutex_t mCSection;
};
union MtPoint
{
    u64 xy;
    struct { u32 x, y; };
};
struct MtLineSegment
{
    alignas(16) MtVector3 p0;
    alignas(16) MtVector3 p1;
};
struct MtHalf4
{
    s64 x, y, z, w;
};
struct MtHalf2
{
    s64 x, y;
};
struct MtString
{
    s32 mRefCount;
    u32 mLength;
    char mString[1];
};


template<class T = void>
class MtDTI
{
public:
    cstring mName;
    MtDTI<T>* mpNext;
    MtDTI<T>* mpChild;
    MtDTI<T>* mpParent;
    MtDTI<T>* mpLink;

    union 
    {
        struct
        {
            u32 mSize : 23;
            u32 mAllocatorIndex : 6;
            u32 mAttr : 3;
        };
        u32 mFlags;
    };
    
    u32 mCRC;

    virtual ~MtDTI() = 0;
    virtual T* NewInstance() = 0;
    virtual T* CtorInstance(T* obj) = 0;
    virtual T* CtorInstanceArray(T* obj, s64 count) = 0;

    size_t GetSize() const
    {
        return static_cast<u64>(mSize) << 2;
    }
    bool InheritsFrom(const std::string& type) const
    {
        if (type == mName)
            return true;

	    for(auto dti = this->mpParent; dti != nullptr && dti->mName != nullptr; dti = dti->mpParent)
	    {
            if (type == dti->mName)
                return true;
	    }

        return false;
    }
    bool InheritsFrom(u32 crc) const
    {
        if (crc == mCRC)
            return true;

        for (auto dti = this->mpParent; dti != nullptr; dti = dti->mpParent)
        {
            if (crc == dti->mCRC)
                return true;
        }

        return false;
    }
    template<class X>
    bool InheritsFrom(const MtDTI<X>* other) const
    {
        if (other->mCRC == mCRC)
            return true;

        for (auto dti = this->mpParent; dti != nullptr; dti = dti->mpParent)
        {
            if (other->mCRC == dti->mCRC)
                return true;
        }

        return false;
    }

    static MtDTI<T>* GetDTI(const std::string& type)
    {
        return GetDTI(((int(*)(const char*, int))0x1421e5830)(type.c_str(), -1) & 0x7FFFFFFF);
    }

    static MtDTI<T>* GetDTI(u32 crc)
    {
        return reinterpret_cast<MtDTI<T>*(*)(u32)>(0x14216da70)(crc);
    }
};

using AnyDTI = MtDTI<void>;

#ifdef REDEFINE_MINMAX
#define min(a,b)            (((a) < (b)) ? (a) : (b))
#define max(a,b)            (((a) > (b)) ? (a) : (b))
#undef REDEFINE_MINMAX
#endif
