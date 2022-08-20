#pragma once

#include <string>
#include <iostream>

#include "Injector.h"

#ifdef MAPPER_EXPORTS
#define MAPPER_API __declspec(dllexport)
#else
#define MAPPER_API __declspec(dllimport)
#endif

extern "C" MAPPER_API void Map(BYTE* data, int pid);