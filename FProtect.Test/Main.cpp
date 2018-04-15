#include "Main.h"

#include <Windows.h>
#include <stdio.h>

class Testing {
public:
    Testing();
    ~Testing();

    char buzz(void *);
private:

};
Testing::Testing() {}
Testing::~Testing() {}
char Testing::buzz(void *) {
    __asm __emit 0x12
    __asm __emit 0x34
    __asm __emit 0x56

    // https://stackoverflow.com/questions/8121320/get-memory-address-of-member-function
    char(__thiscall Testing::*pBuzz)(void *) = &Testing::buzz;
    uintptr_t *address = (uintptr_t *&)pBuzz;
    //FProtect::FProtect_Begin(reinterpret_cast<uintptr_t *>(address), __func__);
    FProtectBegin(address);

    printf("Alea iacta est.\n");

    FProtectEnd(address);

    __asm __emit 0x56
    __asm __emit 0x34
    __asm __emit 0x12
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90

    return 0xFF;
}

void __declspec(noinline) something() {
    __asm _emit 0x12
    __asm _emit 0x34
    __asm _emit 0x56

    printf("Hello world");

    __asm _emit 0x56
    __asm _emit 0x34
    __asm _emit 0x12
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
}

void fuzz() {
    __asm _emit 0x12
    __asm _emit 0x34
    __asm _emit 0x56

    //FProtect::FProtect_Begin(reinterpret_cast<uintptr_t *>(&fuzz), __func__);
    FProtectBegin(fuzz);

    // For some reason msvc optimizes the first call to the second one...
    printf("%x == ", fuzz);
    printf("%x\n", &fuzz);

    int x = 10;
    printf("Hello, fuzz!\n");
    for(int i = 0; i < 100; i++) {
        x += 10;
    }

    //FProtect::FProtect_End(reinterpret_cast<uintptr_t *>(&fuzz));
    FProtectEnd(fuzz);

    __asm _emit 0x56
    __asm _emit 0x34
    __asm _emit 0x12
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
    __asm _emit 0x90
}

int main(int argc, char **argv) {
    FProtect::FProtect_Init();

    printf("calling fuzz the first time\n");
    fuzz();
    printf("calling fuzz the 2nd time\n");
    fuzz();

    something();

    Testing *t = new Testing();
    t->buzz(nullptr);
    t->buzz(nullptr);

    while(true) {
        if(GetAsyncKeyState(VK_END) & 0x8000)
            break; 

        Sleep(1000);
    }

    FProtect::FProtect_Cleanup();

    return 0;
}