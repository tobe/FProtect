#pragma once

#include <cstdint>

namespace FProtect {
	class Function {
	public:
		// Function start and end address
		uintptr_t StartAddress;
		uintptr_t EndAddress;

		bool Protected;

		// Bytecode size
		uint32_t Size;

		// The number of times the function has been referenced
		uint32_t ReferenceCount;

		// XOR Key
		uint8_t Key;

		// Optional function name
		const char *Name;

		Function() {}

		Function(
			uintptr_t StartAddress,
			uintptr_t EndAddress,
			bool Protected,
			uint32_t Size,
			uint32_t ReferenceCount,
			uint8_t Key,
			char *Name
		) {
			this->StartAddress = StartAddress;
			this->EndAddress = EndAddress;
			this->Protected = Protected;
			this->Size = Size;
			this->ReferenceCount = ReferenceCount;
			this->Key = Key;
			this->Name = Name;
		}
	};
}