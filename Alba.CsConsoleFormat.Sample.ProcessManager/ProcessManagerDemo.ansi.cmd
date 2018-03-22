@echo off
copy Alba.CsConsoleFormat.Sample.ProcessManager.exe ProcessManager.exe
copy Alba.CsConsoleFormat.Sample.ProcessManager.exe.config ProcessManager.exe.config
cls

echo.
echo [30;102m~ ProcessManager help[0m
ProcessManager help
echo.
echo [30;102m~ ProcessManager help list[0m
ProcessManager help list
echo.
echo [30;102m~ ProcessManager help start[0m
ProcessManager help start
echo.
echo [30;102m~ ProcessManager help --all[0m
ProcessManager help --all
echo.
echo [30;102m~ ProcessManager help oops[0m
ProcessManager help oops

echo.
echo [30;102m~ ProcessManager list --withtitle[0m
ProcessManager list --withtitle
echo.
echo [30;102m~ ProcessManager list -n devenv -m .[0m
ProcessManager list -n devenv -m .

echo.
echo [30;102m~ ProcessManager start notepad -a %USERPROFILE%/.gitconfig
ProcessManager start notepad -a %USERPROFILE%/.gitconfig
echo.
echo [30;102m~ ProcessManager start oops[0m
ProcessManager start oops

del ProcessManager.exe
del ProcessManager.exe.config 2> nul
@echo on

pause