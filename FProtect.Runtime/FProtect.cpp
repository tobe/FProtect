#pragma once

#include "FProtect.h"
#include "Logger\Logger.h"
#include <map>
#include <algorithm>
#include <assert.h>
#include <time.h>
#include <memory>

#include <intrin.h>  
#pragma intrinsic(_ReturnAddress)  

namespace FProtect {
    std::map<uintptr_t *, Function *> Functions;
    HANDLE CurrentProcess;

    void FProtect_Init() {
        srand(static_cast<unsigned int>(time(NULL)));

        CurrentProcess = GetCurrentProcess();

        #ifdef FPROTECT_DEBUG
        Logger::Initialize(FPROTECT_LOGMODE);
        #endif
    }

    void FProtect_Begin(uintptr_t *FunctionAddress, const char *FunctionName) {
        // The start of the actual function, after CALL FProtect::FProtect_Begin
        uintptr_t StartAddress  = reinterpret_cast<uintptr_t>(_ReturnAddress());

        // Check if this function has been tampered with
        if(Functions.find(FunctionAddress) == Functions.end()) {
            // It doesn't, add the function into the hashmap
            Function *newFunction = new Function();

            newFunction->StartAddress   = StartAddress;
            newFunction->EndAddress     = 0;
            newFunction->Protected      = false;
            newFunction->Size           = 0;
            newFunction->ReferenceCount = 1;
            newFunction->Key            = 0;
            newFunction->Name           = FunctionName;

            Functions.insert(std::make_pair(FunctionAddress, newFunction));

            #ifdef FPROTECT_DEBUG
            Logger::Write(LoggingLevel::OK, __func__, "Added %s at 0x%x", FunctionName, StartAddress);
            #endif

            return;
        }

        // The function has been already added, it has to be protected.
        auto function = GetFunctionByAddress(FunctionAddress);
        #ifdef FPROTECT_DEBUG
        Logger::Write(LoggingLevel::OK, __func__, "Unprotecting %s for execution", FunctionName);
        #endif

        function->ReferenceCount++;
        Protect(FunctionAddress);

        // No longer protected
        function->Protected = false;
    }

    void FProtect_End(uintptr_t *FunctionAddress) {
        // Find the function from the hashmap
        auto function = GetFunctionByAddress(FunctionAddress);

        // Decrement the reference count
        function->ReferenceCount--;

        // Check if freshly added (no end / bytecode size)
        if(function->EndAddress == 0 || function->Size == 0) {
            // Returns to after CALL
            uintptr_t EndAddress = reinterpret_cast<uintptr_t>(_ReturnAddress());

            // Jump back 5 bytes before CALL and update the struct
            EndAddress -= 0x5;
            function->EndAddress = EndAddress;
            function->Size = EndAddress - function->StartAddress;
        }

        // Not protected yet
        if(!function->Protected) {
            // Ensure the ref count is at 0 to get right opcodes
            if(function->ReferenceCount == 0) {
                Protect(FunctionAddress);
            } else {
                #ifdef FPROTECT_DEBUG
                Logger::Write(LoggingLevel::FAIL, __func__, "Reference count mismatch for %s", function->Name);
                #endif
            }
        }
    }

    void Protect(uintptr_t *FunctionAddress) {
        // Fetch the func
        auto function = GetFunctionByAddress(FunctionAddress);
        assert(function->EndAddress != 0);
        assert(function->Size != 0);

        // Get a current key if it's encrypted, otherwise generate a random one.
        uint8_t key = function->Protected ? function->Key : rand() % 255;

        #ifdef FPROTECT_DEBUG
        Logger::Write(LoggingLevel::OK, __func__, "Key for %s is %x", function->Name, key);
        #endif

        // Adjust privileges
        DWORD dwOldProtect;
        uint8_t vp;
        vp = VirtualProtect(
            reinterpret_cast<LPVOID>(function->StartAddress),
            function->Size,
            PAGE_READWRITE,
            &dwOldProtect
        );
        if(vp == 0) {
            #ifdef FPROTECT_DEBUG
            Logger::Write(LoggingLevel::FAIL, __func__, "Ante VirtualProtect failed with: %x", GetLastError());
            #endif
        }

        // XOR
        for(size_t byte = function->StartAddress; byte < function->EndAddress; byte++) {
            *reinterpret_cast<uint8_t *>(byte) ^= key;
        }

        // Swap protection
        DWORD nullstub;
        vp = VirtualProtect(
            reinterpret_cast<LPVOID>(function->StartAddress),
            function->Size,
            dwOldProtect,
            &nullstub // Lost 2 hours on this, gg MS.
        );
        if(vp == 0) {
            #ifdef FPROTECT_DEBUG
            Logger::Write(LoggingLevel::FAIL, __func__, "Post VirtualProtect failed with: %x", GetLastError());
            #endif
        }

        // Flush the CPU cache
        FlushInstructionCache(
            CurrentProcess,
            reinterpret_cast<LPCVOID>(function->StartAddress),
            function->Size
        );

        #ifdef FPROTECT_DEBUG
        Logger::Write(LoggingLevel::OK, __func__, "Done with %s", function->Name);
        #endif

        // The function is now protected
        function->Protected = true;
        function->Key = key;
    }

    void FProtect_Cleanup() {
        // Remove all dynamically allocated objects from the collection
        for(auto &function : Functions) {
            delete function.second;
        }

        Functions.clear();
    }

    BOOL VirtualProtect(LPVOID lpAddress, DWORD dwSize, DWORD flNewProtect, PDWORD lpflOldProtect) {
        // TODO: Make a safer version of this, using syscalls
        return ::VirtualProtect(lpAddress, dwSize, flNewProtect, lpflOldProtect);
    }

    Function *GetFunctionByAddress(uintptr_t *FunctionAddress) {
        // Find the function from the hashmap
        auto functionIterator = Functions.find(FunctionAddress);
        assert(functionIterator->first != nullptr);

        return functionIterator->second;
    }
}