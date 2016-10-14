set echo off
copy *.html ..\bin
copy *.xml  ..\bin
copy *.xlsx ..\bin
copy *.xls  ..\bin

csc program.cs /platform:x86 /unsafe /out:..\bin\program.exe
