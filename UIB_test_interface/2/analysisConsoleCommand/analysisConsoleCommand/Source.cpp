#define _CRT_SECURE_NO_WARNINGS
#define _UNICODE 1
#define UNICODE 1

#include <cstdio>
#include <windows.h>
#include <detours.h>
#include <iostream>

#define A 0
#define W 1

BOOL(WINAPI *pReadConsoleA) (HANDLE hConsoleInput, LPVOID lpBuffer, DWORD nNumberOfCharsToRead, LPDWORD lpNumberOfCharsRead, PCONSOLE_READCONSOLE_CONTROL pInputControl) = ReadConsoleA;
BOOL(WINAPI *pReadConsoleW) (HANDLE hConsoleInput, LPVOID lpBuffer, DWORD nNumberOfCharsToRead, LPDWORD lpNumberOfCharsRead, PCONSOLE_READCONSOLE_CONTROL pInputControl) = ReadConsoleW;

const char* commanReadConsoleA[27] = {
	"tasklist",
	"ver",
	"nslookup",
	"ipconfig /all",
	"net time",
	"systeminfo",
	"netstat -an",
	"qprocess",
	"query user",
	"whoami",
	"net start",
	"time /t",
	"dir",
	"net view",
	"net use",
	"ping",
	"type",
	"net user", 
	"net localgroup",
	"net group", 
	"net config",
	"net share", 
	"dsquery",
	"nbtstat -a",
	"net session",
	"reg",
	"iexplore"
};

const wchar_t* commandReadConsoleW[27] = {
	L"tasklist",
	L"ver",
	L"nslookup",
	L"ipconfig /all",
	L"net time",
	L"systeminfo",
	L"netstat -an",
	L"qprocess",
	L"query user",
	L"whoami",
	L"net start",
	L"time /t",
	L"dir",
	L"net view",
	L"net use",
	L"ping",
	L"type",
	L"net user",
	L"net localgroup",
	L"net group",
	L"net config",
	L"net share",
	L"dsquery",
	L"nbtstat -a",
	L"net session",
	L"reg",
	L"iexplore"
};


void console(LPVOID lpBuffer, INT flag) 
{
	int tmp = -1;
	if (flag == A) 
	{
		for (int i = 0; i < 27; i++) 
		{
			LPSTR command = (LPSTR)lpBuffer;
			if (strstr(_strlwr(command), commanReadConsoleA[i]) != NULL) 
				tmp = i; 
		}
	}
	else if (flag == W) 
	{
		for (int i = 0; i < 27; i++) 
		{
			LPWSTR command = (LPWSTR)lpBuffer;
			if (wcsstr(_wcslwr(command), commandReadConsoleW[i]) != NULL) 
				tmp = i; 
		}
	}

	if (tmp >= 0) 
	{
		wchar_t msg[500] = L"Command: \n";
		wcscat_s(msg, commandReadConsoleW[tmp]);
		MessageBoxW(NULL, msg, L"DANGER", MB_OK | MB_ICONSTOP);
		exit(0);
	}
}

BOOL WINAPI MyReadConsoleA(_In_     HANDLE  hConsoleInput, _Out_    LPVOID  lpBuffer, _In_     DWORD   nNumberOfCharsToRead, _Out_    LPDWORD lpNumberOfCharsRead, _In_opt_ PCONSOLE_READCONSOLE_CONTROL pInputControl)
{
	BOOL ret = pReadConsoleA(hConsoleInput, lpBuffer, nNumberOfCharsToRead, lpNumberOfCharsRead, pInputControl);
	console(lpBuffer, A);
	return ret;
}

BOOL WINAPI MyReadConsoleW(_In_     HANDLE  hConsoleInput, _Out_    LPVOID  lpBuffer, _In_     DWORD   nNumberOfCharsToRead, _Out_    LPDWORD lpNumberOfCharsRead, _In_opt_ PCONSOLE_READCONSOLE_CONTROL  pInputControl)
{
	BOOL ret = pReadConsoleW(hConsoleInput, lpBuffer, nNumberOfCharsToRead, lpNumberOfCharsRead, pInputControl);
	console(lpBuffer, W);
	return ret;
}


INT APIENTRY DllMain(HMODULE hDLL, DWORD Reason, LPVOID Reserved) 
{
	switch (Reason) {
	case DLL_PROCESS_ATTACH: {
		DisableThreadLibraryCalls(hDLL);
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
	
		DetourAttach(&(PVOID&)pReadConsoleA, MyReadConsoleA);
		DetourAttach(&(PVOID&)pReadConsoleW, MyReadConsoleW);

		DetourTransactionCommit();
		break;
	}
	case DLL_PROCESS_DETACH: {
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		
		DetourDetach(&(PVOID&)pReadConsoleA, MyReadConsoleA);
		DetourDetach(&(PVOID&)pReadConsoleW, MyReadConsoleW);

		DetourTransactionCommit();
		break;
	}
	}
	return TRUE;
}