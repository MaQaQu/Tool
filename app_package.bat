del .\YouiToolkit\bin\Release\net48\*.config
del .\YouiToolkit\bin\Release\net48\*.yaml
del .\YouiToolkit\bin\Release\net48\*.pdb
del .\YouiToolkit\bin\Release\net48\*.xml
rd /s /q .\YouiToolkit\bin\Release\net48\logs\
@echo package started with 7z at %date:~,4%.%date:~5,2%.%date:~8,2% %time:~,2%:%time:~3,2%:%time:~6,2%.
cd .\YouiToolkit\bin\Release
ren net48 YouiToolkit
..\..\..\YouiToolkit.Tools\7-Zip\7z a ..\..\..\YouiToolkit_build%date:~,4%%date:~5,2%%date:~8,2%.7z .\YouiToolkit\ -t7z -mx=5 -mf=BCJ2 -r -y
ren YouiToolkit net48
@pause
