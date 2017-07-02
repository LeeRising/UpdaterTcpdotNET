SET NEWLINE=^& echo.

attrib -r %WINDIR%\system32\drivers\etc\hosts   

FIND /C /I "activate.adobe.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO %NEWLINE%^127.0.0.1                   activate.adobe.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "practivate.adobe.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1                   practivate.adobe.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "lmlicenses.wip4.adobe.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1                   lmlicenses.wip4.adobe.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "lm.licenses.adobe.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1                   lm.licenses.adobe.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "na1r.services.adobe.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1                   na1r.services.adobe.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "hlrcv.stage.adobe.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^127.0.0.1                   hlrcv.stage.adobe.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "onhax.net" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                  onhax.net>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "www.onhax.net" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   www.onhax.net>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "https://forum.onhax.net" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   https://forum.onhax.net>>%WINDIR%\system32\drivers\etc\hosts


FIND /C /I "www.masterkreatif.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   www.masterkreatif.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "cloudanna.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   cloudanna.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "piratecity.net" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   piratecity.net>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "www.fullstuff.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   www.fullstuff.com>>%WINDIR%\system32\drivers\etc\hosts

FIND /C /I "fullsoft24u.com" %WINDIR%\system32\drivers\etc\hosts
IF %ERRORLEVEL% NEQ 0 ECHO ^128.199.121.125                   fullsoft24u.com>>%WINDIR%\system32\drivers\etc\hosts


attrib +r %WINDIR%\system32\drivers\etc\hosts   