@echo off
copy Alba.CsConsoleFormat.Sample.ProcessManager.clp22.exe ProcessManager.clp22.exe
copy Alba.CsConsoleFormat.Sample.ProcessManager.clp22.exe.config ProcessManager.clp22.exe.config
cls

echo.
echo [30;102m~ ProcessManager help[0m
ProcessManager.clp22 help
echo.
echo [30;102m~ ProcessManager help list[0m
ProcessManager.clp22 help list
echo.
echo [30;102m~ ProcessManager help start[0m
ProcessManager.clp22 help start
echo.
echo [30;102m~ ProcessManager help-all[0m
ProcessManager.clp22 help-all
echo.
echo [30;102m~ ProcessManager help oops[0m
ProcessManager.clp22 help oops

echo.
echo [30;102m~ ProcessManager list --withtitle[0m
ProcessManager.clp22 list --withtitle
echo.
echo [30;102m~ ProcessManager list -n devenv -m .[0m
ProcessManager.clp22 list -n devenv -m .

echo.
echo [30;102m~ ProcessManager start notepad -a %USERPROFILE%/.gitconfig[0m
ProcessManager.clp22 start notepad -a %USERPROFILE%/.gitconfig
echo.
echo [30;102m~ ProcessManager start oops[0m
ProcessManager.clp22 start oops

del ProcessManager.clp22.exe
del ProcessManager.clp22.exe.config
@echo on

pause