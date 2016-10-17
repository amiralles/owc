set echo off
mkdir ..\bin
copy *.html ..\bin
copy *.xml  ..\bin
copy *.xlsx ..\bin
copy *.xls  ..\bin
cls
csc program.cs /platform:x86 /unsafe /out:..\bin\program.exe
