#include "Logger.h"

namespace FProtect {
    std::ofstream Logger::Filestream;
    bool Logger::ToStdout;

    void Logger::Initialize(LoggingMode Mode, char *File) {
        // (Re)set the mode if file logging is included but a file is not set
        Mode = ((Mode == 0x11) && !File) ? LoggingMode::STDOUT : Mode;

        // Setup some flags
        if((Mode & 0x10) == 1) // File included
            Logger::Filestream.open(File, std::ios::out | std::ios::trunc);
        if((Mode & 0x01) == 1) // Stdout included
            Logger::ToStdout = true;
    }

    void Logger::Write(LoggingLevel Level, const char *Routine, const char *Message, ...) {
        char szBuffer[256];
        va_list args;
        va_start(args, Message);
        vsnprintf(szBuffer, 255, Message, args);
        va_end(args);

        std::string tab("    ");

        // Print out the date first
        auto t = std::time(nullptr);
        #pragma warning(disable: 4996)
        auto tm = *std::localtime(&t);

        std::ostringstream oss;

        switch(Level) {
            case OK:
            oss << "[+] ";
            break;
            case FAIL:
            oss << "[-] ";
            break;
        }

        oss << std::put_time(&tm, "%H:%M:%S ") << Routine << "()" << tab << szBuffer;

        if(Logger::Filestream.good())
            Logger::Filestream << oss.str() << std::endl;

        if(Logger::ToStdout)
            std::cout << oss.str() << std::endl;
    }
}