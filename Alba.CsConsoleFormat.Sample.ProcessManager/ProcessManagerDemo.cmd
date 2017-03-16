@echo off
copy Alba.CsConsoleFormat.Sample.ProcessManager.exe ProcessManager.exe
cls

echo.
echo ~ ProcessManager help
ProcessManager help
echo.
echo ~ ProcessManager help list
ProcessManager help list
echo.
echo ~ ProcessManager help start
ProcessManager help start
echo.
echo ~ ProcessManager help --all
ProcessManager help --all
echo.
echo ~ ProcessManager help oops
ProcessManager help oops

echo.
echo ~ ProcessManager list --withtitle
ProcessManager list --withtitle
echo.
echo ~ ProcessManager list -n devenv -m .
ProcessManager list -n devenv -m .

echo.
echo ~ ProcessManager start notepad -a %USERPROFILE%/.gitconfig
ProcessManager start notepad -a %USERPROFILE%/.gitconfig
echo.
echo ~ ProcessManager start oops
ProcessManager start oops

del ProcessManager.exe
@echo on
