pushd "%~dp0"
cscript.exe //NoLogo lnk.vbs /add "%~dp0/ADMINstart_injdrv.bat.lnk" "%~dp0/start_injdrv.bat" "" "" "" "1" "" ""
cscript.exe //NoLogo lnk.vbs /add "%~dp0/ADMINstop_injdrv.bat.lnk" "%~dp0/stop_injdrv.bat" "" "" "" "1" "" ""