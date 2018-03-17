#include "Main.h"

#include <Windows.h>
#include <stdio.h>

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

    while(true) {
        if(GetAsyncKeyState(VK_SPACE) & 0x8000)
            break;

        Sleep(1000);
    }

    return 0;
}