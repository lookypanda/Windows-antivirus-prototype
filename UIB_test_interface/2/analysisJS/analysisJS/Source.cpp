#define _CRT_SECURE_NO_WARNINGS
#define _UNICODE 1
#define UNICODE 1

#include <cstdio>
#include <windows.h>
#include <detours.h>
#include <wininet.h>
#include <curl.h>
#include <iostream>
#include <map>
#include <string>
#include <tchar.h>

#include <fstream>

#pragma comment(lib, "wininet.lib")
#pragma comment(lib, "urlmon.lib")
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "wldap32.lib")
#pragma comment(lib, "crypt32.lib")
#pragma comment(lib, "normaliz.lib")

using namespace std;
#define A 0
#define W 1

HINTERNET(WINAPI *pHttpOpenRequestW) (HINTERNET hConnect, LPCWSTR lpszVerb, LPCWSTR lpszObjectName, LPCWSTR lpszVersion, LPCWSTR lpszReferrer, LPCWSTR *lplpszAcceptTypes, DWORD dwFlags, DWORD_PTR dwContext) = HttpOpenRequestW;
BOOL(WINAPI *pInternetReadFile) (HINTERNET hFile, LPVOID lpBuffer, DWORD dwNumberOfBytesToRead, LPDWORD lpdwNumberOfBytesRead) = InternetReadFile;
BOOL(WINAPI *pInternetQueryDataAvailable) (HINTERNET hFile, LPDWORD lpdwNumberOfBytesAvailable, DWORD dwFlags, DWORD_PTR dwContext) = InternetQueryDataAvailable;
BOOL(WINAPI *pInternetCloseHandle)(HINTERNET hInternet) = InternetCloseHandle;

#define SIZE 1024
struct site {
	HINTERNET hash;
	std::string info;
} site_list[SIZE];

int cur = 0;

//https://docs.microsoft.com/en-us/windows/win32/wininet/retrieving-http-headers
BOOL html_js(HINTERNET hHttp)
{
	BOOL ret = FALSE;
	LPVOID lpOutBuffer = NULL;
	DWORD dwSize = 0;

retry:
	// This call will fail on the first pass, because no buffer is allocated.
	if (!HttpQueryInfoA(hHttp, HTTP_QUERY_CONTENT_TYPE, (LPVOID)lpOutBuffer, &dwSize, NULL))
	{
		if (GetLastError() == ERROR_HTTP_HEADER_NOT_FOUND)
		{
			// Code to handle the case where the header isn't available.
			return ret;
		}
		else
		{
			// Check for an insufficient buffer.
			if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
			{
				// Allocate the necessary buffer.
				lpOutBuffer = new char[dwSize];
				// Retry the call.
				goto retry;
			}
			else
			{
				// Error handling code.
				if (lpOutBuffer)
				{
					delete[] lpOutBuffer;
				}
				return ret;
			}
		}
	}

	std::string buf = (LPCSTR)lpOutBuffer;
	if (buf.find("javascript") != std::string::npos || buf.find("text/html") != std::string::npos) 
		ret = TRUE; 

	if (lpOutBuffer)
	{
		delete[] lpOutBuffer;
	}

	return ret;
}


BOOL analysisInfo(std::string content) 
{
	DWORD ret = 0;
	STARTUPINFO si;
	PROCESS_INFORMATION pi;
	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));
	WCHAR cmd[32] = L"C:\\Windows\\System32\\cmd.exe";
	WCHAR args[90] = L"/C python C:\\Users\\Julia\\Desktop\\5\\classifier.py C:\\Users\\Julia\\Desktop\\5\\data.tmp";
	LPTSTR szCmdline = _tcsdup(args);
	LPTSTR szPath = _tcsdup(cmd);
	if (CreateProcessW(szPath, szCmdline, NULL, NULL, FALSE, 0, NULL, L"C:\\Users\\Julia\\Desktop\\5", &si, &pi))
	{
		WaitForSingleObject(pi.hProcess, INFINITE);
		GetExitCodeProcess(pi.hProcess, &ret);
		CloseHandle(pi.hProcess);
		CloseHandle(pi.hThread);
	}
	free(szCmdline);
	free(szPath);
	remove("C:\\Users\\Julia\\Desktop\\5\\data.tmp");
	return ret;
}


HINTERNET MyHttpOpenRequestW(HINTERNET hConnect, LPCWSTR lpszVerb, LPCWSTR lpszObjectName, LPCWSTR lpszVersion, LPCWSTR lpszReferrer, LPCWSTR *lplpszAcceptTypes, DWORD dwFlags, DWORD_PTR dwContext)
{
	HINTERNET res = pHttpOpenRequestW(hConnect, lpszVerb, lpszObjectName, lpszVersion, lpszReferrer, lplpszAcceptTypes, dwFlags, dwContext);
	if (res != NULL) 
	{ 
		site_list[cur].hash = res;
		site_list[cur].info = "";
		cur++;
	}
	return res;
}

BOOL MyInternetReadFile(HINTERNET hFile, LPVOID lpBuffer, DWORD dwNumberOfBytesToRead, LPDWORD lpdwNumberOfBytesRead)
{
	BOOL res = pInternetReadFile(hFile, lpBuffer, dwNumberOfBytesToRead, lpdwNumberOfBytesRead);
	if (res != 0 && html_js(hFile) != FALSE) 
	{ 
		for (int i = 0; i < cur; i++)
		{
			if (site_list[i].hash == hFile)
			{
				site_list[i].info += (LPSTR)lpBuffer;
			}
		}
	}
	return res;
}


BOOL MyInternetQueryDataAvailable(HINTERNET hFile, LPDWORD lpdwNumberOfBytesAvailable, DWORD dwFlags, DWORD_PTR dwContext)
{
	int msgboxID = IDNO;
	DWORD NumberOfBytes = *lpdwNumberOfBytesAvailable;
	BOOL res = pInternetQueryDataAvailable(hFile, lpdwNumberOfBytesAvailable, dwFlags, dwContext);
	if (res != 0 && NumberOfBytes != 0 && (*lpdwNumberOfBytesAvailable) == 0) 
	{
		string data; 
		for (int i = 0; i < cur; i++)
		{
			if (site_list[i].hash == hFile)
			{
				data = site_list[i].info;
			}
		}
		
		if (data.size() != 0) 
		{ 
			std::ofstream fout("C:\\Users\\Julia\\Desktop\\5\\data.tmp");
			fout << data;
			fout.close();

			if (analysisInfo(data) != 0)
			{
				msgboxID = MessageBoxW(NULL, L"FOUND A POTENTIAL THREAT!", L"DANGER", MB_YESNO | MB_ICONERROR););
			}
		}
	}
	if (msgboxID == IDYES)
	{
		exit(0);
	}
	return res;
}


BOOL MyInternetCloseHandle(HINTERNET hInternet) 
{
	try 
	{ 
		for (int i = 0; i < cur; i++)
		{
			if (site_list[i].hash == hInternet)
			{
				site_list[i].hash = NULL;
				site_list[i].info = "";
			}
		}
	}
	catch (...) {};
	return pInternetCloseHandle(hInternet);
}




INT APIENTRY DllMain(HMODULE hDLL, DWORD Reason, LPVOID Reserved) 
{
	switch (Reason) {
	case DLL_PROCESS_ATTACH: {
		DisableThreadLibraryCalls(hDLL);
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		
		DetourAttach(&(PVOID&)pHttpOpenRequestW, MyHttpOpenRequestW);
		DetourAttach(&(PVOID&)pInternetReadFile, MyInternetReadFile);
		DetourAttach(&(PVOID&)pInternetQueryDataAvailable, MyInternetQueryDataAvailable);
		DetourAttach(&(PVOID&)pInternetCloseHandle, MyInternetCloseHandle);

		DetourTransactionCommit();
		break;
	}
	case DLL_PROCESS_DETACH: {
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		
		DetourDetach(&(PVOID&)pHttpOpenRequestW, MyHttpOpenRequestW);
		DetourDetach(&(PVOID&)pInternetReadFile, MyInternetReadFile);
		DetourDetach(&(PVOID&)pInternetQueryDataAvailable, MyInternetQueryDataAvailable);
		DetourDetach(&(PVOID&)pInternetCloseHandle, MyInternetCloseHandle);

		DetourTransactionCommit();
		break;
	}
	}
	return TRUE;
}