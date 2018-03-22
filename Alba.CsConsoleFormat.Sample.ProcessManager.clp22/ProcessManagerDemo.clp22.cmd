@echo off
copy Alba.CsConsoleFormat.Sample.ProcessManager.clp22.exe ProcessManager.clp22.exe
copy Alba.CsConsoleFormat.Sample.ProcessManager.clp22.exe.config ProcessManager.clp22.exe.config
cls

echo.
echo ~ ProcessManager help
ProcessManager.clp22 help
echo.
echo ~ ProcessManager help list
ProcessManager.clp22 help list
echo.
echo ~ ProcessManager help start
ProcessManager.clp22 help start
echo.
echo ~ ProcessManager help-all
ProcessManager.clp22 help-all
echo.
echo ~ ProcessManager help oops
ProcessManager.clp22 help oops

echo.
echo ~ ProcessManager list --withtitle
ProcessManager.clp22 list --withtitle
echo.
echo ~ ProcessManager list -n devenv -m .
ProcessManager.clp22 list -n devenv -m .

echo.
echo ~ ProcessManager start notepad -a %USERPROFILE%/.gitconfig
ProcessManager.clp22 start notepad -a %USERPROFILE%/.gitconfig
echo.
echo ~ ProcessManager start oops
ProcessManager.clp22 start oops

del ProcessManager.clp22.exe
del ProcessManager.clp22.exe.config
@echo on

pause