#pragma once

#include "dti_types.h"
#include <functional>

#include "dti_types.h"
#include "MtStream.h"

#pragma execution_character_set("utf-8")

struct sMain {
	using JobHandle = u64;

	u8 mPadding0[0x30];
	u64 mTimer;
	u64 mTimerEx;
	u8 mPadding1[0x8];
	u32 mFrameTimer;
	u8 mPadding2[0x4];
	f32 mFps;
	f32 mMaxFps;
	u8 mPadding3[0x4];
	f32 mActualFps;
	f32 mRealFps;
	u8 mPadding4[0x22];
	bool mPauseNextFrame;
	u8 mPadding5[0x3];
	bool mDeltaTimeAdjust;
	u8 mPadding6[0x1];
	f32 mDeltaTime;
	f32 mDeltaTimeBorder;
	f32 mDeltaTimeLimite;
	f32 mDeltaSec;
	f32 mGlobalSpeed;
	f32 mDelayFrame;
	u8 mPadding7[0x28];
	u32 mDelayJobThreadNum;
	u64 mDelayJobWritePt;
	u64 mDelayJobReadPt;
	u8 mPadding8[0xaf590];
	u32 mCPUCaps;
	u32 mCPUCoreNum;
	u32 mCPULogicalProcessorNum;
	u32 get_mSluggerReleaseVersion() { return std::function<u32(sMain*)>((u32(*)(sMain*))0x141193FEC)(this); }
	void set_mSluggerReleaseVersion(u32 val) { return std::function<void(sMain*, u32)>((void(*)(sMain*, u32))0x14023D5D0)(this, val); }
	u32 get_mWorldReleaseVersion() { return std::function<u32(sMain*)>((u32(*)(sMain*))0x141193FE4)(this); }
	void set_mWorldReleaseVersion(u32 val) { return std::function<void(sMain*, u32)>((void(*)(sMain*, u32))0x14023D5D0)(this, val); }
	u32 get_mGameVersion() { return std::function<u32(sMain*)>((u32(*)(sMain*))0x141193FDC)(this); }
	void set_mGameVersion(u32 val) { return std::function<void(sMain*, u32)>((void(*)(sMain*, u32))0x14023D5D0)(this, val); }
	u32 get_mJobThreadNum() { return std::function<u32(sMain*)>((u32(*)(sMain*))0x141A0EF80)(this); }
	void set_mJobThreadNum(u32 val) { return std::function<void(sMain*, u32)>((void(*)(sMain*, u32))0x14227CA40)(this, val); }
	bool get_mFrameWait() { return std::function<bool(sMain*)>((bool(*)(sMain*))0x140F2BE20)(this); }
	void set_mFrameWait(bool val) { return std::function<void(sMain*, bool)>((void(*)(sMain*, bool))0x14227CA00)(this, val); }
	template<class _T> _T& at(size_t _Off) { return *(_T*)(u64(this) + _Off); }

	static sMain* get() {
		return *(sMain**)0x145224688;
	}

	JobHandle add_delay_job(MtObject* pthis, MT_MFUNC32 pfunc, u32 param) {
		const auto _add_delay_job = (JobHandle(*)(sMain*, JobHandle*, MtObject*, MT_MFUNC32, u32))0x142279410;
		JobHandle handle = 0;
		_add_delay_job(this, &handle, pthis, pfunc, param);

		return handle;
	}

	virtual ~sMain() = 0;
	virtual void* create_ui(void* prop) const = 0;
	virtual bool is_enable_instance() const = 0;
	virtual void create_property() const = 0;
	virtual MtDTI<sMain>* get_dti() const = 0;
	virtual void reset() = 0;
	virtual void move() = 0;
	virtual void create_menu(void* menu) const = 0;
	virtual void apply_world_offset(vector4* offset, vector4* absolute_offset) = 0;
	virtual void close() = 0;
	virtual void final() = 0;
	virtual u64 _vf58() = 0;
	virtual u64 _vf60() = 0;
	virtual u64 _vf68() = 0;
	virtual bool save(MtStream* out) const = 0;
	virtual bool load(MtStream* in) = 0;
	virtual bool save_config(MtStream* out) const = 0;
	virtual bool load_config(MtStream* in) = 0;
	virtual u32 job_loop(void* ph) = 0;
	virtual u32 delay_job_loop(void* ph) = 0;
};
dti_offset_assert(sMain, mFps, 0x58);
