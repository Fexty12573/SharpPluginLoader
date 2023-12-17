#pragma once
typedef unsigned char undefined;
typedef unsigned char undefined1;
typedef unsigned short undefined2;
typedef unsigned int undefined4;
typedef unsigned long long undefined8;
typedef unsigned char byte;
typedef unsigned long long ulonglong;
typedef long long longlong;
typedef unsigned int uint;
typedef unsigned short ushort;
typedef unsigned int u32;

namespace MH {
    namespace Quest {
        inline undefined8(*ErrorCheck)() = (undefined8(*)())0x14114e510;
        inline undefined8(*CheckProgress)() = (undefined8(*)())0x14135e260;
        inline undefined8(*GetCategory)(undefined8) = (undefined8(*)(undefined8))0x1419875c0;
        inline void* OptionalIdList = (void*)0x1432a7cb0;
        inline bool(*CheckComplete)(longlong, uint) = (bool(*)(longlong, uint))0x14135e1e0;
        inline undefined8(*OptionalCount)() = (undefined8(*)())0x141365290;
        inline uint(*OptionalAt)(undefined*, int) = (uint(*)(undefined*, int))0x141365280;
        inline bool(*IsMasterRank)(undefined8, undefined8, undefined8, undefined8) = (bool(*)(undefined8, undefined8, undefined8, undefined8))0x1419885c0;
        inline void* GlobalOptionalQuestList = (void*)0x145073258;
        inline bool(*StarCategoryCheck)(int, int, int) = (bool(*)(int, int, int))0x140f347b0;
        namespace QuestData {
            inline void* ResourceVtable = (void*)0x143442ac0;
            inline undefined8(*ResourceFunc)() = (undefined8(*)())0x141a22de0;
            inline void* ResourcePtr = (void*)0x14506ba18;
        }
        namespace QuestNoList {
            inline void* ResourceVtable = (void*)0x142fdde48;
            inline undefined8(*ResourceFunc)() = (undefined8(*)())0x140479500;
            inline void* ResourcePtr = (void*)0x144d17e20;
        }
    }
    namespace Player {
        inline void* BasePtr = (void*)0x145073e80;
        inline undefined* (*GetPlayer)(undefined*) = (undefined * (*)(undefined*))0x141ba9280;
    }
    namespace Savefile {
        inline bool(*CheckFlag)(longlong, uint) = (bool(*)(longlong, uint))0x14136bdf0;
    }
    namespace Monster {
        inline void(*dtor)(void*) = (void(*)(void*))0x141ca3a10; // 15.20
        inline void* vptr = (void*)0x1434a7a20;
        namespace DamageBehavior {
            inline undefined8(*NextAction)() = (undefined8(*)())0x1413966e0; // 15.20
        }
        inline bool(*LaunchAction)(undefined*, uint) = (bool(*)(undefined*, uint))0x141cc4590; // 15.20
        inline void* (*ctor)(void*, u32, u32) = (void* (*)(void*, u32, u32))0x141ca1130; // 15.20
        namespace SoftenTimers {
            inline undefined8(*WoundPartLocal)() = (undefined8(*)())0x140aff490;
        }
        inline undefined8(*MotionFromId)() = (undefined8(*)())0x141c1afa0;
        inline undefined8(*GetEmName)(uint) = (undefined8(*)(uint))0x1413af000;
        inline undefined8(*GenerateFilePaths)() = (undefined8(*)())0x141ceadc0;
        inline void* EmNameList = (void*)0x143f51c40;
    }
    namespace Weapon {
        namespace UI {
            inline undefined8(*CalcAwakenedElement)() = (undefined8(*)())0x1419900b0;
            inline uint(*CalcRawBloat)(uint, uint) = (uint(*)(uint, uint))0x141990080;
            inline void* RawBloatMultipliers = (void*)0x143410b78;
            namespace Unkn {
                inline undefined8(*ConditionalElementalBloat)() = (undefined8(*)())0x14192cc00;
            }
            inline undefined8(*CalcElementalBloat)() = (undefined8(*)())0x1419900a0;
        }
    }
    namespace EmSetter {
        inline undefined8(*CreateMonster)() = (undefined8(*)())0x141a7f4e0;
    }
    namespace List {
        inline void(*IncreaseCapacity)(void*, ulonglong) = (void(*)(void*, ulonglong))0x140249d10;
    }
    namespace GameVersion {
        inline undefined8(*CalcNum)() = (undefined8(*)())0x1418e61d0;
        inline void* StringPtr = (void*)0x143f4d1d0;
    }
    namespace String {
        inline undefined8(*Format__)(undefined8, undefined8, undefined8, undefined8, undefined) = (undefined8(*)(undefined8, undefined8, undefined8, undefined8, undefined))0x1404649a0;
        inline undefined8(*Format)() = (undefined8(*)())0x140306110;
        inline undefined8(*sPrintf)() = (undefined8(*)())0x14024c520;
    }
    namespace Chat {
        inline undefined8(*BuildShowGameMessage_)() = (undefined8(*)())0x1419b5430;
        inline void** MainPtr = (void**)0x14506d340;
        inline void(*ShowGameMessage)(void*, const char*, float, uint, undefined1) = (void(*)(void*, const char*, float, uint, undefined1))0x141a671f0;
        namespace Unkn {
            inline undefined8(*CallBuild)() = (undefined8(*)())0x1411c0c30;
        }
        inline undefined8(*BuildShowGameMessage)() = (undefined8(*)())0x1419b54d0;
    }
    namespace Damage {
        inline bool(*ApplySoftenBuildup)(longlong, longlong, float) = (bool(*)(longlong, longlong, float))0x1402c84d0;
    }
    namespace File {
        inline undefined8(*LoadResource)() = (undefined8(*)())0x14223ef10;
    }
    namespace QuestBoard {
        inline undefined8(*FilterQuestList)() = (undefined8(*)())0x141157940;
    }
}