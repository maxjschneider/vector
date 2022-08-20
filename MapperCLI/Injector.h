#pragma once

#include <Windows.h>
#include <TlHelp32.h>
#include <iostream>

using f_LoadLibraryA = HINSTANCE(WINAPI*)(const char* lpLibFilename);
using f_GetProcAddress = FARPROC(WINAPI*)(HMODULE hModule, LPCSTR lpProcName);
using f_DLL_ENTRY_POINT = BOOL(WINAPI*)(void* hDll, DWORD dwReason, void* pReserved);

struct MANUAL_MAPPING_DATA {
	f_LoadLibraryA		pLoadLibraryA;
	f_GetProcAddress	pGetProcAddress;
	BYTE*				pbase;
	HINSTANCE			hMod;
};

void Inject(HANDLE hProc, BYTE* pSrcData);