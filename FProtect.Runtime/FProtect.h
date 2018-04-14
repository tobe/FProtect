#pragma once

#include <Windows.h>
#include "Function.h"

#include <memory>

#define FPROTECT_DEBUG 1
#define FPROTECT_LOGMODE LoggingMode::STDOUT

namespace FProtect {
    void FProtect_Init();
    void FProtect_Begin(uintptr_t *FunctionAddress, const char *FunctionName);
    void FProtect_End(uintptr_t *FunctionAddress);
    void FProtect_Erase();
    void FProtect_Restore();
    void FProtect_Cleanup();

    void Protect(uintptr_t *FunctionAddress);
    BOOL VirtualProtect(LPVOID lpAddress, DWORD dwSize, DWORD flNewProtect, PDWORD lpflOldProtect);
    Function *GetFunctionByAddress(uintptr_t *FunctionAddress);
}

#define FProtectBegin(Function) FProtect::FProtect_Begin(reinterpret_cast<uintptr_t *>(Function), __func__);
#define FProtectEnd(Function)   FProtect::FProtect_End(reinterpret_cast<uintptr_t *>(Function));