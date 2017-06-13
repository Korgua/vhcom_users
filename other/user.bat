@echo off
color 02
chcp 1252
cls
echo **************************************
echo *                                    *
echo *    Gyors felhasznalo atallitas     *
echo *                                    *
echo **************************************

set /p irsz=Telephely iranyitoszama:  

setlocal
for /f "tokens=4-5 delims=. " %%i in ('ver') do set VERSION=%%i.%%j
if "%version%" == "10.0" goto :windows10
if "%version%" == "6.3" goto :windows7
if "%version%" == "6.2" goto :windows7
if "%version%" == "6.1" goto :windows7
goto :eof
endlocal

:update_settings
echo Frissites beallitasa: Csak letoltes de nincs telepites
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update" /v AUOptions /t REG_DWORD /d 3 /f
echo Szolgaltatas engedelyezese es inditasa
sc config wuauserv start= auto
echo sc stop start
echo Frissitesek keresese... 
echo Manualisan kell ellenorizni a letoltes allapotat
C:\Windows\system32\wuauclt.exe /detectnow
goto :eof 

:windows7
echo Windows 7,8,2008,2012 specifikus beallitasok
call :update_settings
pause
cls
call :vhcom_users
cls
set name=pénztár
call :editUserInfo
cls
set name=iroda
call :editUserInfo
cls
call :checkNetUse
pause
cls
call :autologon
cls
goto :eof

:windows10

echo Windows 10 specifikus beallitasok
echo.
echo Windows Update letiltasa
sc stop wuauserv
echo Windows Update leallitva
sc config wuauserv start= disabled
echo Windows Update letiltva
cls
call :vhcom_users
cls
set name=pénztár
call :editUserInfo
cls
set name=iroda
call :editUserInfo
cls
call :checkNetUse
cls
call :autologon
pause
cls
goto :eof

:vhcom_users
echo VHCom-os felhasznalok letrehozasa, vagy atnevezese, jelszavak beallitasa
set name=vhcom
call :editUserInfo
set name=vhlocal
call :editUserInfo
set name=whsupport
call :editUserInfo
echo Rendszergazda fiok letiltasa
wmic useraccount where name='Rendszergazda' set disabled=true >NUL
goto :eof

:letrehozas
if /I %name%==vhlocal (
	echo vhlocal profil letrehozasa es beallitasa
	net user vhlocal Pecs-1267 /expires:never /active:yes /passwordchg:no /add
	WMIC USERACCOUNT WHERE Name='%name%' SET PasswordExpires=FALSE
	net localgroup "Rendszergazdák" vhlocal /add >NUL
	net localgroup "Felhasználók" vhlocal /delete >NUL
	goto :eof
	)
if /i %name%==whsupport (
	echo whsupport profil letrehozasa es beallitasa
	net user %name% Pecs_7621 /expires:never /active:yes /passwordchg:no /add
	WMIC USERACCOUNT WHERE Name='%name%' SET PasswordExpires=FALSE
	net localgroup "Rendszergazdák" %name% /add >NUL
	net localgroup "Felhasználók" %name% /delete >NUL
	net localgroup "Asztal távoli felhasználói" %name% /add >NUL
	goto :eof
	)
set /p a=A(z) %name% nem letezik. Letrehozzam? [i,n]  
if /i %name%==iroda set newpsw=Iroda%irsz%
if /i %name%==pénztár set newpsw=P%irsz%
if /i %a%==i (
	echo a %name% jelszava: %newpsw%
	net user %name% %newpsw% /expires:never /active:yes /passwordchg:no /add
	WMIC USERACCOUNT WHERE Name='%name%' SET PasswordExpires=FALSE
	set /p s=Az %name% felhasznalonak kell tavoli asztal? [i,n]  
	if /i "%s%"==i net localgroup "Asztal távoli felhasználói" %name% /add >NUL
	)

goto :eof

:editUserInfo
wmic useraccount get name | findstr /i "%name%" > bla.txt
chcp 852 >NUL
set /p nameEx=<bla.txt
chcp 1252 >NUL
del bla.txt
if "%nameEx%"=="" (
	goto :letrehozas
	)
if /I %nameEx%==%name% (
	call :confirm
	) else goto :letrehozas
goto :eof


:confirm
setlocal
set decrease=false
if /i %name%==pénztár (
	set psw=P%irsz%
	set decrease=true
	)
if /i %name%==iroda (
	set psw=Iroda%irsz%
	set decrease=true
	)
if /i %name%==vhcom set psw=Pecs_1267
if /i %name%==vhlocal set psw=Pecs-1267
if /I %name%==vhcom (
	echo VHcom fiok atnevezese vhsupport-ra
	wmic useraccount where name='vhcom' rename vhsupport >NUL
	wmic useraccount where name='vhsupport' set PasswordExpires=FALSE >NUL
	net user vhsupport /fullname:vhsupport
	)
if /I %decrease%==true echo Ha a(z) %name% rendszergazda lenne, onnan most torlesre kerül

if /I %decrease%==true (
	2>nul net localgroup "Rendszergazdák" %name% /delete >NUL
	2>nul net localgroup "Asztal távoli felhasználói" %name% /delete >NUL
	2>nul net localgroup "Felhasználók" %name% /delete >NUL
	net localgroup "Felhasználók" %name% /add >NUL

	)

if /I %name%==vhcom set name=vhsupport
echo %name% jelszavanak beallitasa a kovetkezore: %psw%
net user %name% %psw%
pause
endlocal
goto :eof


:autologon
cls
chcp 852 >NUL
wmic useraccount get name
echo A fenti felhasznalok kozül melyik jelentkezzen be automatikusan?
echo A mar meglevo automatikus bejelentkezes megmarad
echo A kis es nagybetü nincs megkulonboztetve!
set /p login=Irj be egy 0-t, ha egyik sem:  
if /I %login% NEQ 0 echo a megadott iranyitoszam: %irsz%
if /I %login% NEQ 0 set /p passw=Jelszava:
if /I %login% NEQ 0 (
	REG ADD "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v AutoAdminLogon /t REG_SZ /d 1 /f
	REG ADD "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v DefaultDomainName /t REG_SZ /d 127.0.0.1 /f
	REG ADD "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v DefaultUserName /t REG_SZ /d %login% /f
	REG ADD "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v DefaultPassword /t REG_SZ /d %passw% /f
	)
goto :eof

:checkNetUse
reg query HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\run | findstr /I "gazda vhcom" >bla.txt
set /p regIsOk=<bla.txt
if /i %regIsOk NEQ "" (
	cls
	color 04
	echo.
	echo.
	echo.       *******************************************************************
	echo.       *                                                                 *
	echo.       *  A gepen Rendszergazda vagy VHCom alol erheto el az OfficeLine  *
	echo.       *   Utvonal: HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\run   *
	echo.       *                                                                 *
	echo.       *******************************************************************
	echo.
	echo.
	pause
	)
del bla.txt
cls
color 02
goto :eof