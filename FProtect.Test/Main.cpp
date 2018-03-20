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
    // https://stackoverflow.com/questions/8121320/get-memory-address-of-member-function
    char(__thiscall Testing::*pBuzz)(void *) = &Testing::buzz;
    uintptr_t *address = (uintptr_t *&)pBuzz;
    //FProtect::FProtect_Begin(reinterpret_cast<uintptr_t *>(address), __func__);
    FProtectBegin(address);

    printf("Alea iacta est.\n");

    FProtectEnd(address);
    return 0xFF;
}

void fuzz() {
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
}

int main(int argc, char **argv) {
    FProtect::FProtect_Init();

    printf("calling fuzz the first time\n");
    fuzz();
    printf("calling fuzz the 2nd time\n");
    fuzz();

    Testing *t = new Testing();
    t->buzz(nullptr);
    t->buzz(nullptr);

    while(true) {
        if(GetAsyncKeyState(VK_END) & 0x8000)
            break; 

        Sleep(1000);
    }

    return 0;
}