#pragma once

#include "dti/dti_types.h"

#include <d3d11.h>
#include <dxgi.h>

struct sMhResource;
class sQuest;
struct cUserData;
struct sUserData;
struct cGUIObjSizeAdjustMessage;
struct cGUIObjChildAnimationRoot;
class MtWeaponInfo;
struct uGimmick;
struct cDraw;
struct ActionInfo;

struct tagPOINT;
typedef tagPOINT* LPPOINT;

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
typedef unsigned long long QWORD;
typedef unsigned int uint32_t;

#define DECLARE_FUNCTION(NAMESPACE, NAME, ADDRESS, RETURN, ...)           \
    namespace NAMESPACE {                                                 \
    inline RETURN (*NAME)(__VA_ARGS__) = (RETURN(*)(__VA_ARGS__))ADDRESS; \
    }
#define DECLARE_POINTER(NAMESPACE, TYPE, NAME, ADDRESS)\
    namespace NAMESPACE {\
    inline TYPE NAME = (TYPE)ADDRESS;\
    }

/* Does other stuff as well, did not research what inidividual parameters do */
DECLARE_FUNCTION(MH::Player, DrunkBird, 0x140f2b800, ulonglong, undefined8, undefined8)
// 41 F7 E0 C1 EA 05 6B C2 64 44 2B C0 33 DB 8B D3   (15.10)

/* arg1: pointer to savedata
* arg2: amount of points to be given
* ret:  unknown */
DECLARE_FUNCTION(MH::Player, ApplyHRPoints, 0x14137a500, undefined8, void*, int)
// 8B 8F 9C 00 00 00 B8 FF E0 F5 05 03 CE 3B C8 0F 47 C8 89 8F 9C 00 00 00 48 8D 4F 08

/* Returns the pointer of the players current target (with target camera)
arg1: unknown
arg2: id of target (0, 1, 2...)
ret:  Pointer to target monster */
DECLARE_FUNCTION(MH::Player, CheckCurrentTarget, 0x1412ad630, longlong, longlong, uint32_t)
// 48 83 EC 38 4C 8B C1 83 FA 0E 73 7F 8B C2   (15.10)

/* Not updated */
DECLARE_FUNCTION(MH::Player, ManageSharpness, 0x142134b70, void, longlong, int)

/* Needs more testing, not updated */
DECLARE_FUNCTION(MH::Player, PlayerAction, 0x14026a8e0, ulonglong, longlong, int*)

DECLARE_FUNCTION(MH::Player, SetPosition, 0x141c1f1c0, void, void*, float*, float*)

DECLARE_FUNCTION(MH::Player, SetActionSet, 0x141c186a0, void, void*, u32, void*, u32, u32)

DECLARE_FUNCTION(MH::Player, CreateGimmick, 0x141f933a0, void, void*, u32, float)

DECLARE_FUNCTION(MH::Player, HandleActionPacket, 0x1411afae0, void, void*, void*, void*)

DECLARE_FUNCTION(MH::Player, GetPlayerFromOnlineID, 0x141b41660, void*, void*, u32, s32)
DECLARE_FUNCTION(MH::Player, GetUserDataFromOnlineID, 0x141b412f0, void*, void*, u32, s32)
DECLARE_FUNCTION(MH::Player, GetPlayerFromSteamID, 0x141b41110, void*, void*, u64)

DECLARE_FUNCTION(MH::Player, ChangeWeapon, 0x141f76ce0, void, void*, u32, u32)

DECLARE_FUNCTION(MH::Player, dtor, 0x142032120, void, void*, u32)

DECLARE_POINTER(MH::Player, void*, StaticExePointer, 0x14506d270)

namespace MH::Player {
inline void* GetInstance() {
    void* instance = nullptr;
    void* sActBtn = *(void**)StaticExePointer;

    if (sActBtn)
        instance = *(void**)(ulonglong(sActBtn) + 0x80);

    return instance;
}
}

namespace MH::Player {
inline void* GetPlayerSingleton() {
    return *reinterpret_cast<void**>(0x14500ca60); // 0x14506f1b0 (15.11)
}
}
/* Saves the game
NOTE: May or may not have a 3rd parameter
arg1: unknown
arg2: unknown
ret:  unknown */
DECLARE_FUNCTION(MH::Savefile, Save, 0x141b3fbc0, undefined8, longlong, longlong*)
// 48 8B 03 48 8D 57 48 41 B8 04 00 00 00 48 89 5C 24 60   (15.10)

/* Function isn't appropriately titled, does other things regarding savedata
arg1: destination (unknown)
arg2: source (source + 0x98 -> Selected save slot, -1 if when nothing)
ret:  unknown */
DECLARE_FUNCTION(MH::Savefile, SelectSaveSlot, 0x141b55900, longlong, longlong, longlong)
// C6 46 3F 00 48 8D 4F 78 8B 47 48 48 8D 53 78 89 43 48   (15.10)

DECLARE_FUNCTION(MH::Savefile, GetSelectSave, 0x141ba9280, cUserData*, sUserData*)

namespace MH::Savefile {
inline sUserData* GetInstance() {
    return *reinterpret_cast<sUserData**>(0x145073e80);
}
}
/* Used for monster flinch actions, can be used to overwrite anything the monster is currently doing
arg1: [monster + 0x12278]
arg2: [[monster + 0x12278] + 0x4B0] */
DECLARE_FUNCTION(MH::Monster::DamageBehavior, NextAction_, 0x1413a6bf0, void, void*, void*)

/* Not in use anymore, forgot parameters */
DECLARE_FUNCTION(MH::Monster::DamageBehavior, EmDmgStuff, 0x1402be5d0, undefined8, longlong, ulonglong, uint, uint*)
// @param this (nEmAI::DieBehavior)
// @param mon (uAIEm)
DECLARE_FUNCTION(MH::Monster::DieBehavior, OnDeath, 0x1413a7130, void, void*, void*)

DECLARE_FUNCTION(MH::Monster::DamageCheck, HandleDamagePacket, 0x1402c4090, void, void*, void*)
DECLARE_FUNCTION(MH::Monster::DamageCheck, DeserializeDamagePacket, 0x1402c1f20, void, void*, void*, void*)

// @param cEmDamageCheck* this
// @param EmDamageInfo1* in1
// @param u32 unk1
// @param u32 unk2
// @param u32 unk3
// @param EmDamageInfo2* in2
DECLARE_FUNCTION(MH::Monster::DamageCheck, ProcessDamagePacket, 0x1402bc450, void, void*, void*, u32, u32, u32, void*)

// TODO: Look into this
DECLARE_FUNCTION(MH::Monster, SetupThks, 0x141bd20a0, void, void)

DECLARE_FUNCTION(MH::Monster, GetPartInfo, 0x141cb66b0, void*, void*, u32)

DECLARE_FUNCTION(MH::Monster, Initialize, 0x1422456f0, void, void*)

DECLARE_FUNCTION(MH::Monster, AI_Interpreter, 0x141343650, longlong*, longlong, ulonglong, undefined8)
// 45 33 F6 40 32 FF 44 89 75 67 4D 8B E0   (15.10)

DECLARE_FUNCTION(MH::Monster, ProcessTHKSegment, 0x141349980, int, void*, int*, void*)
// 40 38 af 60 05 00 00 74 11 40 88 af 60 05 00 00 41 8d 44 24 fb

DECLARE_FUNCTION(MH::Monster, MonsterTimer, 0x141bb7d80, void, longlong, undefined8, ulonglong, undefined8)
// 48 8B F9 0F 85 A3 02 00 00    (15.10)

/* Read a given .col file and apply it to the appropriate monster
arg1: destination where collision values are written to (monster object, eventually)
arg2: source, aka the collision file that was loaded into memory. Both are void* / char* most likely */
DECLARE_FUNCTION(MH::Monster, ReadMonsterCol, 0x1419c5820, void, longlong, longlong)
// 8B 42 08 89 41 08 8B 42 0C 89 41 0C 8B 42 18 89 41 18 8B 42 20   (15.10)


DECLARE_FUNCTION(MH::Monster, AITargeting, 0x1413334f0, void, longlong, ulonglong, ulonglong)

/* Returns the given Monsters current Target
arg1: monster who's target is to be retrieved. [monster + 0x12278],
ret:  pointer to current target of arg1 */
DECLARE_FUNCTION(MH::Monster, GetTargetMonster, 0x141bc9720, longlong, void*)
// 48 8B 81 C0 0A 00 00 48 85 C0 74 06 F6 40 0C 0E 75 02 (15.10)

/* Used by the game for in-combat targeting (and in some other situations too
arg1: longlong _this_ai_data: The AI Data of the monster who's target is to be checked/changed (is technically void*)
arg2 is unused, arg3 is irrelevant */
DECLARE_FUNCTION(MH::Monster, KeepTrackOfTarget, 0x141343030, void, void*, undefined8, byte)
// 48 89 5C 24 58 0F 57 C0 48 89 7C 24 70   (15.10)

DECLARE_FUNCTION(MH::Monster, AICheckEnemyState, 0x141346FB0, void*, void*, void*)

/* Don't know params anymore */
DECLARE_FUNCTION(MH::Monster, UpdateTarget, 0x142244ad0, void, longlong, longlong*)

/* Check Monster via (param_1 - 0x1D700 == monster)													UPDATED UP UNTIL HERE
arg1: target, the monster to perform the operation for (monster + 0x1D700), void*
arg2: unknown, likely an index for something
arg3: coords, an array of XYZ coordinates that will be set. (Vec3) */
DECLARE_FUNCTION(MH::Monster, SetTargetCoord, 0x1413a40b0, void, void*, int, float*)

/* Enrage Monster
arg1: pointer to structure owned by monster to be enraged (monster + 0x1BD08)
ret:  whether function succeeded or not */
DECLARE_FUNCTION(MH::Monster, Enrage, 0x1402a8da0, bool, longlong)

/* Unenrage Monster
arg1: same as enrage */
DECLARE_FUNCTION(MH::Monster, Unenrage, 0x1402a9030, void, longlong)

DECLARE_FUNCTION(MH::Monster, EnableActionLinkingForNextActions, 0x1413a1040, void, bool*, int)
DECLARE_FUNCTION(MH::Monster, ActionsConnectable, 0x141cd4900, bool, void*, int, int, int*)

DECLARE_FUNCTION(MH::Monster, GetActionTableAt, 0x141390be0, void*, u32)
DECLARE_FUNCTION(MH::HealthManager, SetHP, 0x141221fd0, void, void*, float, bool)
DECLARE_FUNCTION(MH::HealthManager, SetMaxHP, 0x1412220a0, void, void*, float)
DECLARE_FUNCTION(MH::Quest, AcceptQuest, 0x141b7e1e0, void, sQuest*, u32, u8)
DECLARE_FUNCTION(MH::Quest, EnterQuest, 0x141b62900, void, sQuest*)
DECLARE_FUNCTION(MH::Quest, LeaveQuest, 0x141b7fa30, void, sQuest*)
DECLARE_FUNCTION(MH::Quest, AbandonQuest, 0x141b8caf0, void, sQuest*, u32)
DECLARE_FUNCTION(MH::Quest, ReturnFromQuest, 0x141b8b6f0, void, sQuest*)
DECLARE_FUNCTION(MH::Quest, ExitQuest, 0x141b806d0, void, sQuest*)
DECLARE_FUNCTION(MH::Quest, DepartOnQuest, 0x141b847a0, void, sQuest*, bool)

DECLARE_FUNCTION(MH::Quest, SetRewards, 0x141ebdf20, u32, void*, void*, void*, u32)
DECLARE_FUNCTION(MH::EmSetter, CatHelperSetter, 0x141b33d10, void, longlong)
/* Process shoutouts given by the player
arg1: unknown
arg2: unknown
arg3: shoutout id
arg4: unknown
arg5: unknown */
DECLARE_FUNCTION(MH::Chat, Shoutouts, 0x141a656d0, void, __int64, int, unsigned int, unsigned int, int)
// Shoutouts: 48 8B F1 72 24 4C 8B 91 B0 9A 00 00   (15.10)

DECLARE_FUNCTION(MH::Chat, PrintMessage, 0x1423606f0, void, cGUIObjSizeAdjustMessage*, const char*, u32)
DECLARE_FUNCTION(MH::ActionController, DoAction, 0x140269c90, bool, void*, ActionInfo*)
DECLARE_FUNCTION(MH::ActionController, ResizeActionSet, 0x14026aba0, void, void*, u32, u32, void*)
DECLARE_FUNCTION(MH::ActionController, SetActionSet, 0x141bfc1b0, void, void*, u32, void*, u32, int)
DECLARE_FUNCTION(MH::ActionController, InitializeAction, 0x14026a830, void*, void*, ActionInfo*, MtDTI<>*)
/* Loads any file from within nativePC/chunks into the game and returns a pointer to it
arg1: sMhResource    Current Location of the file manager (see below)
arg2: objDef			Resource definition, there is one for each file type within the exe
arg3: filename		Path to the file without the extension
arg4: flags			To tell the function to do certain stuff differently, generally 1 is used
ret:  pointer to where in memory the file was loaded to */
DECLARE_FUNCTION(MH::File, _LoadResource, 0x14223ef10, undefined8, void*, void*, char*, char)
DECLARE_FUNCTION(MH::File, Func, 0x1418c0ce0, ulonglong, uint)

/* dereference to get location of file manager */
DECLARE_POINTER(MH::File, void**, FileManager, 0x145183f40)
DECLARE_FUNCTION(MH::Resource, LoadResource, 0x1422214f0, void*, void*, void*, char*, char)
DECLARE_FUNCTION(MH::Resource, TryUnloadResource, 0x1422422b0, void, void* sMhResource, void* instance)

DECLARE_POINTER(MH::Resource, void**, StaticExePointer, 0x1451217c0)

namespace MH::Resource {
template<class T>
T* GetResource(MtDTI<T>* dti, const char* path, u32 flags) {
    return (T*)LoadResource(*StaticExePointer, (void*)dti, const_cast<char*>(path), flags);
}
}
/* Applies Damage
arg1: this - cEmDamageCheck
arg2: unk
arg3: { Monster, unk, unk, ... }
*/
DECLARE_FUNCTION(MH::cEmDamageCheck, ApplyDamage, 0x1402c1c80, void, void*, void*, void*)
// arg1: Address of shell file/object (For monster only)
//inline longlong(*Execute)(void*, long long*, unsigned int*, int*, long long) = (longlong(*)(void*, long long*, unsigned int*, int*, long long))0x141abbd40;
DECLARE_FUNCTION(MH::Shell, dtor, 0x141a301a0, void*, void*, char)
DECLARE_FUNCTION(MH::Shell, ExecuteForPlayer, 0x141aba910, void, void*, void*, void*, int*)
DECLARE_FUNCTION(MH::Shell, ExecuteOther, 0x141abaac0, void, void*, void*, void*, int*, int*)
DECLARE_FUNCTION(MH::Shell, ExecuteForMonster, 0x141abbd40, longlong, void*, long long*, unsigned int*, int*, long long)
DECLARE_FUNCTION(MH::Shell, CopyEnvData, 0x140a26450, float*, float* dst, const float* src)
DECLARE_FUNCTION(MH::Palico, PalicoBuff, 0x1419909c0, undefined8, longlong)
DECLARE_FUNCTION(MH::Palico, CheerbongoBuff, 0x1419a39d0, undefined8, longlong, undefined8, undefined8, undefined8)
DECLARE_FUNCTION(MH::Palico, ParaDamage, 0x1411f7780, float, longlong)
DECLARE_FUNCTION(MH::Debug, SpawnMonster, 0x141b31d80, longlong, undefined8, uint, uint)
DECLARE_FUNCTION(MH::Debug, SpawnMonsterInit, 0x141a6e3d0, longlong, undefined8, ulonglong, ulonglong, char)
DECLARE_FUNCTION(MH::Debug, CallSpawnMonsterInit, 0x141a6df10, longlong, undefined8, undefined4)
DECLARE_FUNCTION(MH::Debug, GetControllerState, 0x1422c2140, char, __int64)

DECLARE_POINTER(MH::Debug, undefined8*, SpawnVar, 0x14506D410)

inline char(__fastcall* Calls_CallsCreateMonster)(char*, __int64*) = (char(__fastcall*)(char*, __int64*))0x142124e90;
inline char(__fastcall* CallsCreateMonster)(void*, __int64, int, QWORD*) = (char(__fastcall*)(void*, __int64, int, QWORD*))0x142125050;
inline bool(__fastcall* BigFunction)() = (bool(__fastcall*)())0x1402519f0;

/* Spawns monsters from the quest data list
* arg1: QuestDataList (dereferenced)
* arg2: NULL
* arg3: NULL
* arg4: NULL
* ret:  true/false based on success of the function */
inline bool(__fastcall* SpawnQuestMonster)(undefined8, unsigned int, unsigned int, unsigned int) = (bool(__fastcall*)(undefined8, unsigned int, unsigned int, unsigned int))0x141b889b0;
inline char* (__fastcall* GetMonsterSobjPath)(longlong, char*, ulonglong) = (char* (__fastcall*)(longlong, char*, ulonglong))0x141b68660;
DECLARE_FUNCTION(MH::Debug, GetMonsterSobjPathExped, 0x141b689a0, char*, sQuest*, char*, int, int, int)
DECLARE_POINTER(MH::Debug, void*, QuestDataList, 0x14506f240)
DECLARE_FUNCTION(MH::Camera, UpdateCameraTarget, 0x141fc1bf0, float*, longlong)
DECLARE_FUNCTION(MH::Camera, UpdateCameraPosition, 0x141fbb610, float*, undefined8, float, longlong)
DECLARE_FUNCTION(MH::sMhInputText, Dispatch, 0x14239d640, void, wchar_t*)
/* Gets called multiple times every time a scene is loaded
* arg1: this
* arg2: unknown
* ret: void */
DECLARE_FUNCTION(MH::sMhScene, Unknown, 0x141b2dd10, void, void*, void*)
DECLARE_FUNCTION(MH::sMhScene, ctor, 0x141b2d290, void**, void**)
DECLARE_FUNCTION(MH::sMhScene, dtor, 0x141b2d710, void**, void**, void**)
DECLARE_FUNCTION(MH::sScene, ctor, 0x142280eb0, void**, void**, unsigned int)
DECLARE_FUNCTION(MH::Network::cPacketBase, ctor, 0x1411f8d70, void*, void*)

DECLARE_FUNCTION(MH::Network, SendPacket, 0x141b22c80, void, void*, void*, int, int, int)
DECLARE_FUNCTION(MH::Network, RecvPacket, 0x1424b7fb0, void, void*, __int64, int, char*, int)
DECLARE_FUNCTION(MH::Network, SendBuffer, 0x1424b9b50, void, void*, char*, u32, u32, u32, u32)

DECLARE_FUNCTION(MH::Network, ReceiveQuestBinary, 0x141b0fa80, void, void*, u32, void*, bool, bool)
/* takes this, and flags as parameters */
DECLARE_FUNCTION(MH::sLightLinker::Area, dtor, 0x141ac5d10, void*, void*, char)
DECLARE_FUNCTION(MH::sLightLinker::Area, NewInstance, 0x141acb120, void*, void*, void*)
DECLARE_FUNCTION(MH::rLightLinker, dtor, 0x141a0e0d0, void*, void*, char)
DECLARE_POINTER(MH::rLightLinker, void*, ObjectDef, 0x14506af10)
/* Gets called all the time
* arg1: this
* arg2: unknown
* ret: void */
DECLARE_FUNCTION(MH::sMhGUI, Unknown, 0x141af39c0, void, void*)
DECLARE_FUNCTION(MH::sMhGUI, dtor, 0x141ae74b0, void*, void*, char)
DECLARE_FUNCTION(MH::sMhGUI, ShowDamageNumber, 0x141ae2470, void, void* thisptr, void* entity, u32 number, vector3* hit_pos, u32 color, u32 crit, u32 buff, u32 tenderize, bool unk)
DECLARE_FUNCTION(MH::sMhGUI, FormatDamageNumber, 0x141e196e0, void, void* thisptr, u32 unk1, bool unk2, u32 damage, vector3* hit_pos, u32 color, u32 crit, u32 buff, u32 tenderize, bool unk3)

DECLARE_FUNCTION(MH::sMhGUI, DisplayPopup, 0x141ae2700, void, void*, const char*, float, float, bool, float, float)
DECLARE_FUNCTION(MH::sMhGUI, DisplayYesNoDialog, 0x141ae1380, void, void*, void*)

DECLARE_POINTER(MH::sMhGUI, void*, Vftable, 0x1434513b8)

namespace MH::sMhGUI {
inline void* GetInstance() {
    return *reinterpret_cast<void**>(0x1451c2400); // 0x145224b80 (15.11)
}
}
DECLARE_FUNCTION(MH::sMhMain, ctor, 0x141aebb00, void*, void*)
DECLARE_FUNCTION(MH::sMhMain, dtor, 0x141b02a00, void*, void*, char)
DECLARE_FUNCTION(MH::sMhMain, move, 0x141af1e90, void, void*)

DECLARE_POINTER(MH::sMhMain, void*, Vftable, 0x143452bc0)
DECLARE_POINTER(MH::sMhMain, void**, StaticExePointer, 0x145224688)
DECLARE_FUNCTION(MH::Gimmick, ctor, 0x141d01330, void**, void**)
DECLARE_FUNCTION(MH::Gimmick, Spawn, 0x1416fbb60, uGimmick*, u32 gmType, u32 unk, void* source)
DECLARE_FUNCTION(MH::Gimmick, GenNew, 0x1402cb1d0, void, void*, int, int, float*, u64, int) // 0x1402cbe50 (15.11)
DECLARE_FUNCTION(MH::Gimmick, Init, 0x1402ccab0, void, void*, uGimmick*, int, vector3*, int, bool, int*, bool)
DECLARE_FUNCTION(MH::Gimmick, PostInit, 0x141d0dba0, void, uGimmick*)
DECLARE_FUNCTION(MH::nActEm043::LargeBite, ctor, 0x140c2f280, void**, void**)
DECLARE_FUNCTION(MH::MtObject, CtorInstance, 0x14218d380, void**, unsigned __int64, void**)
DECLARE_FUNCTION(MH::MtObject, NewInstance, 0x14218d340, void**, void)
DECLARE_FUNCTION(MH::AK::SoundEngine, RegisterGameObj, 0x1429133e0, int, unsigned __int64, const char*)
DECLARE_FUNCTION(MH::AK::SoundEngine, RegisterGameObj2, 0x142913350, int, unsigned __int64)
inline void(*DetermineDamageNumberType)(void*, unsigned int, void*, unsigned int, unsigned int, unsigned int, unsigned int, unsigned char, unsigned int)
= (void(*)(void*, unsigned int, void*, unsigned int, unsigned int, unsigned int, unsigned int, unsigned char, unsigned int))0x141ce1670;
DECLARE_FUNCTION(MH::GUI::cGUIObject, SetCurrentFrame, 0x142341920, void, void*, float, bool)
DECLARE_FUNCTION(MH::GUI::cGUIObject, InitProperty, 0x14235e1a0, void, void*, bool)
DECLARE_FUNCTION(MH::GUI, DisplayAlert, 0x1418d5960, void, const char*)
DECLARE_FUNCTION(MH::GUI, DisplayMessageWindow, 0x141ae3220, void, void*, const char*, void*, float*, u8)

DECLARE_FUNCTION(MH::GUI, SetIconRect, 0x1418d22b0, void, cGUIObjChildAnimationRoot*, u32, u32, bool, bool, u64)
DECLARE_FUNCTION(MH::rModel, Deserialize, 0x14227eee0, bool, void*, void*)
DECLARE_FUNCTION(MH::Weapon, GetWeaponInfo, 0x142182000, MtWeaponInfo*, u32)
DECLARE_FUNCTION(MH::Render, D3D11CreateDevice, 0x142795c08, HRESULT, IDXGIAdapter*, D3D_DRIVER_TYPE, HMODULE, UINT, const D3D_FEATURE_LEVEL*, UINT, UINT, ID3D11Device**, D3D_FEATURE_LEVEL*, ID3D11DeviceContext**)
DECLARE_FUNCTION(MH::Render, SetDrawPass, 0x1423ea7d0, void, cDraw*, u32)
DECLARE_FUNCTION(MH::Render, Present, 0x1423a8eb0, bool, void*, UINT)

DECLARE_FUNCTION(MH::uGUITitle, play, 0x141efec30, void, void*)

DECLARE_FUNCTION(MH::rStarCatalogue, Deserialize, 0x14256ce60, bool, void*, void*)

DECLARE_FUNCTION(MH::cpAnimationlayer, Update, 0x14224c150, void, void*, int, u32, void*, void*, void*, void*, void*)
