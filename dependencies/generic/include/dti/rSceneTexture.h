#pragma once

#include "dti_types.h"
#include <functional>

#pragma execution_character_set("utf-8")

struct rSceneTexture {
	u8 mPadding0[0x5c];
	s32 mRefCount;
	u32 mAttr;
	u8 mPadding1[0xc];
	u64 mSize;
	u64 mID;
	u64 mCreateTime;
	u8 mPadding2[0x240];
	u32 mAttributes;
	u32 mWidth;
	u32 mHeight;
	f32 mWidthRate;
	f32 mHeightRate;
	u8 mPadding3[0x4];
	color mClearColor;
	u32 get_mQuality() { return std::function<u32(rSceneTexture*)>((u32(*)(rSceneTexture*))0x1402B7C90)(this); }
	void set_mQuality(u32 val) { return std::function<void(rSceneTexture*, u32)>((void(*)(rSceneTexture*, u32))0x1402B8DF0)(this, val); }
	u32 get_mState() { return std::function<u32(rSceneTexture*)>((u32(*)(rSceneTexture*))0x1404521A0)(this); }
	void set_mState(u32 val) { return std::function<void(rSceneTexture*, u32)>((void(*)(rSceneTexture*, u32))0x140452380)(this, val); }
	u32 get_mTag() { return std::function<u32(rSceneTexture*)>((u32(*)(rSceneTexture*))0x1404521B0)(this); }
	void set_mTag(u32 val) { return std::function<void(rSceneTexture*, u32)>((void(*)(rSceneTexture*, u32))0x140452390)(this, val); }
	cstring get_mPath() { return std::function<cstring(rSceneTexture*)>((cstring(*)(rSceneTexture*))0x141986F00)(this); }
	void set_mPath(cstring val) { return std::function<void(rSceneTexture*, cstring)>((void(*)(rSceneTexture*, cstring))0x14023D5D0)(this, val); }
	template<class _T> _T& at(size_t _Off) { return *(_T*)(u64(this) + _Off); }
};

