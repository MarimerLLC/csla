echo "del Properties\*.resx"
del Properties\*.resx >nul 2>&1
echo "copy ..\Csla.Shared.Resources\*.resx Properties\"
copy ..\Csla.Shared.Resources\*.resx Properties\ 