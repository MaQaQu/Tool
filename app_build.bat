set "binpath_com=Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\"
set "binpath_pro=Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\"
set "binpath_ent=Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\"
set "path=%path%;C:\%binpath_com%"
set "path=%path%;D:\%binpath_com%"
set "path=%path%;E:\%binpath_com%"
set "path=%path%;F:\%binpath_com%"
set "path=%path%;C:\%binpath_pro%"
set "path=%path%;D:\%binpath_pro%"
set "path=%path%;E:\%binpath_pro%"
set "path=%path%;F:\%binpath_pro%"
set "path=%path%;C:\%binpath_ent%"
set "path=%path%;D:\%binpath_ent%"
set "path=%path%;E:\%binpath_ent%"
set "path=%path%;F:\%binpath_ent%"
@echo rebuild started with vs2022ent at %date:~,4%.%date:~5,2%.%date:~8,2% %time:~,2%:%time:~3,2%:%time:~6,2%.
msbuild YouiToolkit.sln /t:Rebuild /p:Configuration=Release
@pause
