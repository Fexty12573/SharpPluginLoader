#pragma once
#include <cstdint>


// TODO(Andoryuuta): AOB scan for these instead?
namespace preloader::address {
	const uint64_t IMAGE_BASE = 0x140000000;
	const uint64_t PROCESS_OEP = IMAGE_BASE + 0x2741668;
	const uint64_t PROCESS_SECURITY_COOKIE = IMAGE_BASE + 0x4bf4be8;
	const uint64_t SECURITY_COOKIE_INIT_GETTIME_RET = IMAGE_BASE + 0x27422e2;
	const uint64_t SCRT_COMMON_MAIN_SEH = IMAGE_BASE + 0x27414f4;
	const uint64_t WINMAIN = IMAGE_BASE + 0x13a4c00;

}  // namespace preloader::address