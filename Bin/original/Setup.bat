@echo off
..\bin\CachePwn.exe 1 cache.bin .\
move /y *.raw ..\intermediate\
move /y *.json ..\intermediate\
copy /y ..\intermediate\0002.raw ..\input
copy /y ..\intermediate\0006.raw ..\input
copy /y ..\intermediate\0008.raw ..\input
copy /y ..\intermediate\0009.raw ..\input
echo Setup Complete.
pause