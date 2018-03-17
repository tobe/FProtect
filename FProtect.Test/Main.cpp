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
    void *address;
    __asm {
        push eax;
        mov eax, (Testing::buzz);
        mov address, eax;
        pop eax;
    }
    FProtectBegin(address);

    printf("Alea iacta est.");

    FProtectEnd(address);
}

void fuzz() {
    FProtectBegin(fuzz);

    int x = 10;
    printf("Hello, fuzz!\n");
    for(int i = 0; i < 100; i++) {
        x += 10;
    } 

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
        if(GetAsyncKeyState(VK_SPACE) & 0x8000)
            break;

        Sleep(1000);
    }

    return 0;
}