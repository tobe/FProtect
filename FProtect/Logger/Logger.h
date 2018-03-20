#pragma once

#include <iostream>
#include <fstream>
#include <iomanip>
#include <ctime>
#include <sstream>
#include <stdarg.h>

namespace FProtect {
    enum LoggingLevel {
        OK,
        FAIL
    };

    enum LoggingMode {
        STDOUT = 1, // 01
        FILE = 16   // 10   11
    };

    class Logger {
    public:
        static void Initialize(LoggingMode mode, char *File = nullptr);
        
        static void Write(LoggingLevel Level, const char *Routine, const char *Message, ...);

    private:
        static std::ofstream Filestream;
        static bool ToStdout;
    };
}