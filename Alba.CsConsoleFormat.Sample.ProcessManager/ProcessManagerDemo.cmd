@echo off
copy Alba.CsConsoleFormat.Sample.ProcessManager.exe ProcessManager.exe
cls

echo ~ ProcessManager help
ProcessManager help
echo ~ ProcessManager help list
ProcessManager help list
echo ~ ProcessManager help start
ProcessManager help start
echo ~ ProcessManager help oops
ProcessManager help oops

echo ~ ProcessManager list --withtitle
ProcessManager list --withtitle
echo ~ ProcessManager list -n devenv -m .
ProcessManager list -n devenv -m .

echo ~ ProcessManager start notepad -a %USERPROFILE%/.gitconfig
ProcessManager start notepad -a %USERPROFILE%/.gitconfig
echo ~ ProcessManager start oops
ProcessManager start oops

del ProcessManager.exe
@echo on
