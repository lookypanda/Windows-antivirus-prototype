'****************************************************************************
'* Script utility for managing shortcut files
'* ver 1.07 2011.07.27
'* Programmed by Polyakov A.N. mailto:xey@yandex.ru
'****************************************************************************
'
' Add Shortcut  - lnk.vbs /add   <shortcut> <target> [args] [work_dir] [icon] [win_style] [hot_key] [descr]
' Get Shortcut  - lnk.vbs /get   <shortcut>
' Get Script    - lnk.vbs /get!  <shortcut>
'   or          - lnk.vbs /getscript <shortcut>
' Find Shortcut - lnk.vbs /find  <folder>   <target_to_find> [args_to_find]

' <shortcut> : [sf:SpecialFolder\]Path\ShortcutFile[.lnk]
' <folder>   : [sf:SpecialFolder\]Path
'
' SpecialFolder :
'   Desktop           - Рабочий стол
'   Favorites         - Избранное
'   Fonts             - Шрифты
'   MyDocuments       - Мои документы
'   NetHood           - Сетевое окружение
'   PrintHood         - Принтеры
'   Programs          - подменю Программы из меню Пуск
'   Recent            - подменю Документы из меню Пуск
'   SendTo            - подменю Отправить из контекстного меню файлов
'   StartMenu         - Главное меню
'   Startup           - Автозагрузка из подменю Программы
'   Templates         - Шаблоны
' Only WinNT/2000/XP/2003 :
'   AllUsersDesktop   - Рабочий стол всех пользователей
'   AllUsersStartMenu - Главное меню всех пользователей
'   AllUsersPrograms  - подменю Программы из меню Пуск всех пользователей
'   AllUsersStartup   - Автозагрузка из подменю Программы всех пользователей

' [icon] : IconFileName,IconIndex
'   Index 0 is first icon in the file

' [win_style] : 3 - Maximize, 4 - Standard, 7 - Minimze

' [hot_key] : ALT+SHIFT+<Chr>,  CTRL+ALT+<Chr>,
'             CTRL+SHIFT+<Chr>, ALT+CTRL+SHIFT+<Chr>

' <target_to_find> : target|rx:RegExped_Target
' [args_to_find]   : args|rx:RegExped_Args

' [args]           special symbols : ^  -> "
'                                    ^^ -> ^

rem BEGIN

' ver 1.07 2011.07.27 - add alias "/getscript" for "/get!" key
' ver 1.06 2010.04.18 - add special symbols ^ for "
' ver 1.05 2009.09.01 - add return code (for %ERRORLEVEL% variable)
' ver 1.04 2008.10.30 - fix <shortcut> parsing; fix SpecialFolder order; create folders for /add
' ver 1.03 2008.08.27 - add /find key
' ver 1.02 2007.06.16 - support URL-links
' ver 1.01 2006.12.08 - check lnk-file existance; change output format
' ver 1.00 2006.01.21

on Error Resume Next

const forRead=1
const forWrite=2

dim lnk_file, url, target, arg, path, shortcut_name, work_dir, icon, win_style
dim shell, shortcut, parentFolder, fso, fo, rx, colFile, objFile, objFldr
dim Q, ext, rx1, rx2

set shell = WScript.CreateObject("WScript.Shell")
set fso = CreateObject("Scripting.FileSystemObject")

outCon=right(LCase(wScript.FullName),11)="cscript.exe"

nl = chr(13) & chr(10)

set pars = WScript.Arguments
cnt=pars.count

if cnt<2 then
   ShowHelp
end if


cmd=LCase(pars(0))

lnk_file=pars(1)

if LCase(left(lnk_file,3))="sf:" then
   arr1=split(pars(1),"\")
   arr2=split(arr1(LBound(arr1)),":")
   lnk_file=replace(pars(1),arr1(LBound(arr1)),shell.SpecialFolders(arr2(LBound(arr2)+1)))
end if


if cmd="/getscript" then cmd="/get!"

if cmd="/find" then
   if cnt<3 then ShowHelp

   folder=lnk_file

   target=LCase(trim(pars(2)))
   rx1=left(target,3)="rx:"
   if rx1 then target=right(target,len(target)-3)

   if cnt>3 then args=LCase(trim(pars(3))) else args=""
   rx2=left(args,3)="rx:"
   if rx2 then args=right(args,len(args)-3)

   if rx1 or rx2 then
      set rx=new RegExp
      rx.IgnoreCase=true
   end if

   set colFile=fso.getFolder(folder).Files
   for each objFile in colFile
     lnk_file=objFile.Name
     ext=LCase(right(lnk_file,3))
     if (ext="lnk") or (ext="url") then
        set shortcut = shell.CreateShortcut(folder & "\" & lnk_file)
        Q=false
        if rx1 then
           rx.pattern=target
           Q=rx.test(LCase(shortcut.TargetPath))
        else
           Q=LCase(shortcut.TargetPath)=target
        end if
        if len(args)>0 then
           if rx2 then
              rx.pattern=args
              Q=Q and rx.test(LCase(shortcut.Arguments))
           else
              Q=Q and LCase(shortcut.Arguments)=args
           end if
        end if
        if Q then WScript.echo folder & "\" & lnk_file
     end if
   next
   WScript.quit
end if

url=false

if cmd="/add" then
   if cnt<3 then ShowHelp

   target=trim(pars(2))
   url=Instr(target,"://")>0

   if not url then
      if cnt>3 then args=repl2(trim(pars(3)),"^^","^","^",chr(34)) else args=""
      if cnt>4 then work_dir=trim(pars(4)) else work_dir=""

      if len(work_dir)=0 then
         set fso = CreateObject("Scripting.FileSystemObject")
         set fo  = fso.getFile(target)
         if (Err.Number<>0) then
            Err.clear
         else
            if fo.type<>"" then
               work_dir=fo.parentFolder
            end if
         end if
      end if

      icon=""
      if cnt>5 then
         icon=trim(pars(5))
      end if

      win_style=4
      if cnt>6 then
         win_style=trim(pars(6))
      end if

      hot_key=""
      if cnt>7 then
         hot_key=UCase(trim(pars(7)))
      end if

      descr=""
      if cnt>8 then
         descr=trim(pars(8))
      end if
   end if
end if


S=LCase(right(lnk_file,4))
if (S<>".lnk") and (S<>".url") then
   S=".lnk"
   if url then
      S=".url"
   end if
   lnk_file=lnk_file & S
end if


QRet=0

Q=fso.FileExists(lnk_file)
set shortcut = shell.CreateShortcut(lnk_file)

select case cmd
  case "/add"
    set objFldr=fso.CreateFolder(fso.GetParentFolderName(lnk_file))
    shortcut.TargetPath=target
    shortcut.IconLocation=icon
    if not url then
       shortcut.Arguments=args
       shortcut.WorkingDirectory=work_dir
       shortcut.IconLocation=icon
       shortcut.WindowStyle=win_style
       shortcut.HotKey=hot_key
       shortcut.Description=descr
    end if
    Err.clear
    shortcut.save
    QRet=Err.Number

  case "/get"
    if Q then
       target=shortcut.TargetPath
       args=shortcut.Arguments
       work_dir=shortcut.WorkingDirectory
       icon=shortcut.IconLocation
       win_style=shortcut.WindowStyle
       hot_key=shortcut.Hotkey
       descr=shortcut.Description
       Error=""
    else
       Error=" ERROR: (FILE DON'T EXIST) "
       QRet=1
    end if

    wscript.echo "Shortcut File   =" & Error & lnk_file
    wscript.echo "TargetPath      =" & target
    wscript.echo "Arguments       =" & args
    wscript.echo "WorkingDirectory=" & work_dir
    wscript.echo "IconLocation    =" & icon
    wscript.echo "WindowStyle     =" & win_style
    wscript.echo "HotKey          =" & hot_key
    wscript.echo "Description     =" & descr

  case "/get!"
    if Q then
       lnk_file=replace(sf(lnk_file),"%","%%")
       target=replace(sf(shortcut.TargetPath),"%","%%")
       args=shortcut.Arguments
       work_dir=replace(sf(shortcut.WorkingDirectory),"%","%%")
       icon=replace(sf(shortcut.IconLocation),"%","%%")
       win_style=shortcut.WindowStyle
       hot_key=shortcut.Hotkey
       descr=shortcut.Description

       S="cscript.exe //NoLogo " & wScript.ScriptName & " /add "
       S=S & qw(lnk_file) & " "
       S=S & qw(target) & " "
       S=S & qw(args) & " "
       S=S & qw(work_dir) & " "
       S=S & qw(icon) & " "
       S=S & qw(win_style) & " "
       S=S & qw(hot_key) & " "
       S=S & qw(descr)
    else
       QRet=1
    end if
    wscript.echo S
end select

wScript.quit(QRet)


sub ShowHelp
   set fso = CreateObject("Scripting.FileSystemObject")
   set fo = fso.GetFile(wScript.ScriptFullName)

   set txt = fo.OpenAsTextStream(forRead)
   S=""
   Q=false
   while not Q
     T=trim(txt.ReadLine)
     Q=(len(T)>0) and (left(T,1)<>"'")
     if not Q then
        if len(T)>0 then S=S & right(T,len(T)-1)
        S=S & vbCrLf
     end if
   wend
   txt.close
   wScript.echo S
   wScript.quit
end sub


function qw(T)
  Ret=chr(34) & T & chr(34)
  qw=Ret
end function


function repl2(SS,old1,new1,old2,new2)
  Ret=""
  if len(old1)<len(old2) then
     C=old2
     old2=old1
     old1=C
     C=new2
     new2=new1
     new1=C
  end if

  arr=split(SS,old1)

  for J=0 to UBound(arr)
    if J>0 then
       Ret=Ret & new1
    end if
    Ret=Ret & replace(arr(J),old2,new2)
  next

  repl2=Ret
end function


function sf(path)
  dim f(16)

  Ret=path

  f(1)= "AllUsersStartup"
  f(2)= "AllUsersPrograms"
  f(3)= "AllUsersStartMenu"
  f(4)= "AllUsersDesktop"
  f(5)= "Startup"
  f(6)= "Programs"
  f(7)= "StartMenu"
  f(8)= "Desktop"
  f(9)= "Favorites"
  f(10)="Fonts"
  f(11)="MyDocuments"
  f(12)="NetHood"
  f(13)="PrintHood"
  f(14)="Recent"
  f(15)="SendTo"
  f(16)="Templates"

  S0=LCase(path)
  set sh=wScript.createObject("wScript.Shell")

  for I=1 to 16
    S1=LCase(sh.SpecialFolders(f(I)))
    if instr(S0,S1)=1 then
       Ret="sf:" & f(I) & right(path,len(S0)-len(S1))
       exit for
    end if
  next
  sf=Ret
end function
