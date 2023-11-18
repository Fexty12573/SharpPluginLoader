#pragma once

#define CIMGUI_DEFINE_ENUMS_AND_STRUCTS
#include <cimgui/cimgui.h>

#define IMGUI_IMPL_API

#ifndef IM_NEW
struct ImNewWrapper {};
inline void* operator new(size_t, ImNewWrapper, void* ptr) { return ptr; }
inline void  operator delete(void*, ImNewWrapper, void*) {} // This is only required so we can use the symmetrical new()
#define IM_ALLOC(_SIZE)                     igMemAlloc(_SIZE)
#define IM_FREE(_PTR)                       igMemFree(_PTR)
#define IM_PLACEMENT_NEW(_PTR)              new(ImNewWrapper(), _PTR)
#define IM_NEW(_TYPE)                       new(ImNewWrapper(), igMemAlloc(sizeof(_TYPE))) _TYPE
template<typename T> void IM_DELETE(T* p) { if (p) { p->~T(); igMemFree(p); } }
#endif

#ifndef IM_ASSERT
#include <assert.h>
#define IM_ASSERT(_EXPR) assert(_EXPR)
#endif

#ifndef IM_ARRAYSIZE
#define IM_ARRAYSIZE(_ARR)          ((int)(sizeof(_ARR) / sizeof(*(_ARR))))
#endif

#pragma region ImVector Extension
#include <string.h>
template<typename T>
struct ImVectorExt {
    int Size;
    int Capacity;
    T* Data;

    // Constructors, destructor
    inline ImVectorExt() { Size = Capacity = 0; Data = NULL; }
    inline ImVectorExt(const ImVectorExt<T>& src) { Size = Capacity = 0; Data = NULL; operator=(src); }
    inline ImVectorExt<T>& operator=(const ImVectorExt<T>& src) { clear(); resize(src.Size); if (src.Data) memcpy(Data, src.Data, (size_t)Size * sizeof(T)); return *this; }
    inline ~ImVectorExt() { if (Data) IM_FREE(Data); } // Important: does not destruct anything


    inline void         clear() { if (Data) { Size = Capacity = 0; IM_FREE(Data); Data = NULL; } }  // Important: does not destruct anything
    inline void         clear_delete() { for (int n = 0; n < Size; n++) IM_DELETE(Data[n]); clear(); }     // Important: never called automatically! always explicit.
    inline void         clear_destruct() { for (int n = 0; n < Size; n++) Data[n].~T(); clear(); }           // Important: never called automatically! always explicit.

    inline bool         empty() const { return Size == 0; }
    inline int          size() const { return Size; }
    inline int          size_in_bytes() const { return Size * (int)sizeof(T); }
    inline int          max_size() const { return 0x7FFFFFFF / (int)sizeof(T); }
    inline int          capacity() const { return Capacity; }
    inline T& operator[](int i) { IM_ASSERT(i >= 0 && i < Size); return Data[i]; }
    inline const T& operator[](int i) const { IM_ASSERT(i >= 0 && i < Size); return Data[i]; }

    inline T* begin() { return Data; }
    inline const T* begin() const { return Data; }
    inline T* end() { return Data + Size; }
    inline const T* end() const { return Data + Size; }
    inline T& front() { IM_ASSERT(Size > 0); return Data[0]; }
    inline const T& front() const { IM_ASSERT(Size > 0); return Data[0]; }
    inline T& back() { IM_ASSERT(Size > 0); return Data[Size - 1]; }
    inline const T& back() const { IM_ASSERT(Size > 0); return Data[Size - 1]; }
    inline void         swap(ImVectorExt& rhs) { int rhs_size = rhs.Size; rhs.Size = Size; Size = rhs_size; int rhs_cap = rhs.Capacity; rhs.Capacity = Capacity; Capacity = rhs_cap; T* rhs_data = rhs.Data; rhs.Data = Data; Data = rhs_data; }

    inline int          _grow_capacity(int sz) const { int new_capacity = Capacity ? (Capacity + Capacity / 2) : 8; return new_capacity > sz ? new_capacity : sz; }
    inline void         resize(int new_size) { if (new_size > Capacity) reserve(_grow_capacity(new_size)); Size = new_size; }
    inline void         resize(int new_size, const T& v) { if (new_size > Capacity) reserve(_grow_capacity(new_size)); if (new_size > Size) for (int n = Size; n < new_size; n++) memcpy(&Data[n], &v, sizeof(v)); Size = new_size; }
    inline void         shrink(int new_size) { IM_ASSERT(new_size <= Size); Size = new_size; } // Resize a vector to a smaller size, guaranteed not to cause a reallocation
    inline void         reserve(int new_capacity) { if (new_capacity <= Capacity) return; T* new_data = (T*)IM_ALLOC((size_t)new_capacity * sizeof(T)); if (Data) { memcpy(new_data, Data, (size_t)Size * sizeof(T)); IM_FREE(Data); } Data = new_data; Capacity = new_capacity; }
    inline void         reserve_discard(int new_capacity) { if (new_capacity <= Capacity) return; if (Data) IM_FREE(Data); Data = (T*)IM_ALLOC((size_t)new_capacity * sizeof(T)); Capacity = new_capacity; }

    // NB: It is illegal to call push_back/push_front/insert with a reference pointing inside the ImVector data itself! e.g. v.push_back(v[10]) is forbidden.
    inline void         push_back(const T& v) { if (Size == Capacity) reserve(_grow_capacity(Size + 1)); memcpy(&Data[Size], &v, sizeof(v)); Size++; }
    inline void         pop_back() { IM_ASSERT(Size > 0); Size--; }
    inline void         push_front(const T& v) { if (Size == 0) push_back(v); else insert(Data, v); }
    inline T* erase(const T* it) { IM_ASSERT(it >= Data && it < Data + Size); const ptrdiff_t off = it - Data; memmove(Data + off, Data + off + 1, ((size_t)Size - (size_t)off - 1) * sizeof(T)); Size--; return Data + off; }
    inline T* erase(const T* it, const T* it_last) { IM_ASSERT(it >= Data && it < Data + Size && it_last >= it && it_last <= Data + Size); const ptrdiff_t count = it_last - it; const ptrdiff_t off = it - Data; memmove(Data + off, Data + off + count, ((size_t)Size - (size_t)off - (size_t)count) * sizeof(T)); Size -= (int)count; return Data + off; }
    inline T* erase_unsorted(const T* it) { IM_ASSERT(it >= Data && it < Data + Size);  const ptrdiff_t off = it - Data; if (it < Data + Size - 1) memcpy(Data + off, Data + Size - 1, sizeof(T)); Size--; return Data + off; }
    inline T* insert(const T* it, const T& v) { IM_ASSERT(it >= Data && it <= Data + Size); const ptrdiff_t off = it - Data; if (Size == Capacity) reserve(_grow_capacity(Size + 1)); if (off < (int)Size) memmove(Data + off + 1, Data + off, ((size_t)Size - (size_t)off) * sizeof(T)); memcpy(&Data[off], &v, sizeof(v)); Size++; return Data + off; }
    inline bool         contains(const T& v) const { const T* data = Data;  const T* data_end = Data + Size; while (data < data_end) if (*data++ == v) return true; return false; }
    inline T* find(const T& v) { T* data = Data;  const T* data_end = Data + Size; while (data < data_end) if (*data == v) break; else ++data; return data; }
    inline const T* find(const T& v) const { const T* data = Data;  const T* data_end = Data + Size; while (data < data_end) if (*data == v) break; else ++data; return data; }
    inline int          find_index(const T& v) const { const T* data_end = Data + Size; const T* it = find(v); if (it == data_end) return -1; const ptrdiff_t off = it - Data; return (int)off; }
    inline bool         find_erase(const T& v) { const T* it = find(v); if (it < Data + Size) { erase(it); return true; } return false; }
    inline bool         find_erase_unsorted(const T& v) { const T* it = find(v); if (it < Data + Size) { erase_unsorted(it); return true; } return false; }
    inline int          index_from_ptr(const T* it) const { IM_ASSERT(it >= Data && it < Data + Size); const ptrdiff_t off = it - Data; return (int)off; }
};
#pragma endregion

#define ImDrawCallback_ResetRenderState     (ImDrawCallback)(-8)
