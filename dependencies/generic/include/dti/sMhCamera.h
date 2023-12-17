#pragma once
#include "dti_types.h"
#include "uMhCamera.h"
#include "rSceneTexture.h"
#include <functional>

#pragma execution_character_set("utf-8")

struct sMhCamera
{
	struct Viewport
	{
		void* vft;
		uMhCamera* mpCamera;
		uMhCamera* mpTestCamera;
		rSceneTexture* mSceneTexture;
		bool mVisible;
		bool mPassThroughCheck;
		u8 mNo;
		u8 mPriority;
		u8 mMode;
		u8 mDisplay;
		MtRect mRegion;
		vector4 alignas(16) mFrustum[6];
		MtMatrix alignas(16) mViewMat;
		MtMatrix alignas(16) mProjMat;
		MtMatrix alignas(16) mPrevViewMat;
		MtMatrix alignas(16) mPrevProjMat;
	};
	dti_size_assert(Viewport, 0x1A0);
	dti_offset_assert(Viewport, mViewMat, 0xA0);
	dti_offset_assert(Viewport, mProjMat, 0xE0);

public:
	void* vft;
	u8 _1[0x48];
	Viewport mViewports[8];
	MtRect mScreenRect;
	MtRect mVirtualRect;
	color mBlankColor;
	color mBGColor;
	bool mPause;
	template<class _T> _T& at(size_t _Off) const { return *(_T*)(u64(this) + _Off); }

	static sMhCamera* get() {
		return *reinterpret_cast<sMhCamera**>(0x1451c21c0);
	}
};

