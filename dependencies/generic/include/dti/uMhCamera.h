#pragma once

#include "dti_types.h"
#include <functional>

#pragma execution_character_set("utf-8")

struct uMhCamera {
	void* vft;
	u8 _1[0x38];
	u64 mUnitGroup;
	u8 _2[0x18];
	u64 mTagBit;
	f32 mDeltaSec;
	f32 mDeltaTime;
	u8 _3[0xc8];
	f32 mFarPlane;
	f32 mNearPlane;
	f32 mAspect;
	f32 mFov;
	u8 _4[0x8];
	vector3 alignas(16) mCameraPos;
	vector3 alignas(16) mCameraUp;
	vector3 alignas(16) mTargetPos;
	string get_TagStr(u32 idx) { return std::function<string(uMhCamera*, u32)>((string(*)(uMhCamera*, u32))0x142245460)(this, idx); }
	void set_TagStr(string val, u32 idx) { return std::function<void(uMhCamera*, string, u32)>((void(*)(uMhCamera*, string, u32))0x142245670)(this, val, idx); }
	u32 get_mBeFlag() { return std::function<u32(uMhCamera*)>((u32(*)(uMhCamera*))0x1402EB620)(this); }
	void set_mBeFlag(u32 val) { return std::function<void(uMhCamera*, u32)>((void(*)(uMhCamera*, u32))0x141191F40)(this, val); }
	u32 get_mMoveLine() { return std::function<u32(uMhCamera*)>((u32(*)(uMhCamera*))0x1402EB6F0)(this); }
	void set_mMoveLine(u32 val) { return std::function<void(uMhCamera*, u32)>((void(*)(uMhCamera*, u32))0x140AFE5A0)(this, val); }
	string get_Name() { return std::function<string(uMhCamera*)>((string(*)(uMhCamera*))0x1422449F8)(this); }
	void set_Name(string val) { return std::function<void(uMhCamera*, string)>((void(*)(uMhCamera*, string))0x14023D5D0)(this, val); }
	u32 get_mDrawView() { return std::function<u32(uMhCamera*)>((u32(*)(uMhCamera*))0x140B111F0)(this); }
	void set_mDrawView(u32 val) { return std::function<void(uMhCamera*, u32)>((void(*)(uMhCamera*, u32))0x140B11600)(this, val); }
	u32 get_mDrawMode() { return std::function<u32(uMhCamera*)>((u32(*)(uMhCamera*))0x1416ABBF0)(this); }
	void set_mDrawMode(u32 val) { return std::function<void(uMhCamera*, u32)>((void(*)(uMhCamera*, u32))0x1422456A0)(this, val); }
	bool get_Fix() { return std::function<bool(uMhCamera*)>((bool(*)(uMhCamera*))0x142245520)(this); }
	void set_Fix(bool val) { return std::function<void(uMhCamera*, bool)>((void(*)(uMhCamera*, bool))0x1422456B0)(this, val); }
	bool get_Draw() { return std::function<bool(uMhCamera*)>((bool(*)(uMhCamera*))0x142245480)(this); }
	void set_Draw(bool val) { return std::function<void(uMhCamera*, bool)>((void(*)(uMhCamera*, bool))0x142134900)(this, val); }
	bool get_Move() { return std::function<bool(uMhCamera*)>((bool(*)(uMhCamera*))0x142245530)(this); }
	void set_Move(bool val) { return std::function<void(uMhCamera*, bool)>((void(*)(uMhCamera*, bool))0x1422456D0)(this, val); }
	template<class _T> _T& at(size_t _Off) { return *(_T*)(u64(this) + _Off); }
};
dti_size_assert(uMhCamera, 0x180);
