#pragma once

#include <Windows.h>
#include "Function.h"

#define FPROTECT_DEBUG 1

namespace FProtect {
    void FProtect_Init();
    void FProtect_Begin(void *FunctionAddress, const char *FunctionName);
    void FProtect_End(void *FunctionAddress);
    void FProtect_Erase();
    void FProtect_Restore();

    void Protect(void *FunctionAddress);
    BOOL VirtualProtect(LPVOID lpAddress, DWORD dwSize, DWORD flNewProtect, PDWORD lpflOldProtect);
    Function *GetFunctionByAddress(void *FunctionAddress);
}

#define FProtectBegin(Function) FProtect::FProtect_Begin(reinterpret_cast<void *>(Function), __func__);
#define FProtectEnd(Function)   FProtect::FProtect_End(reinterpret_cast<void *>(Function));