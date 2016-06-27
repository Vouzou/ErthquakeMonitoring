rem %1 paraffin tool
rem %2 project folder

echo ">>> %1 <<<"
echo ">>> %2 <<<"
cd /d "%2"

set ExpectedOutputDirectory=..\Binaries\Release

IF NOT EXIST %ExpectedOutputDirectory% goto :DirectoryNotFound
IF NOT EXIST %1 goto :ParaffinToolNotFound

set EXECUTABLE_FRAGMENTS=EarthquakeExecutablesFragments
set LIBRARY_FRAGMENTS=EarthquakeLibraryFragments

IF NOT EXIST %EXECUTABLE_FRAGMENTS%.wxs "%1" -v -d %ExpectedOutputDirectory% -groupname EARTHQUAKE_EXECUTABLES -alias %ExpectedOutputDirectory% %EXECUTABLE_FRAGMENTS%.wxs -norecurse -norootdirectory -ext .dll -ext .cab -ext .pdb -ext .xml -ext .txt -ext .xaml -ext .wixpdb -ext .msi -ext .metagen -rex ".*\.vshost\.exe.*" -rex ".*\.vshost\.exe\.manifest.*" -rex ".*\.vshost\.exe\.config.*" -rex ".*\.dll\.config.*"
IF NOT EXIST %LIBRARY_FRAGMENTS%.wxs "%1" -v -d %ExpectedOutputDirectory% -groupname EARTHQUAKE_LIBRARIES -alias %ExpectedOutputDirectory% %LIBRARY_FRAGMENTS%.wxs -norootdirectory -ext .exe -ext .pdb -ext .txt -ext .wixpdb -ext .msi -ext .metagen -rex ".*(?<!(Activity(\w*)Catalog(\w*)))\.xml.*" -rex ".*\.vshost\.exe.*" -rex ".*\.vshost\.exe\.manifest.*" -rex ".*\.vshost\.exe\.config.*" -rex ".*\.exe\.config.*"

%1 -v -update %EXECUTABLE_FRAGMENTS%.wxs
%1 -v -update %LIBRARY_FRAGMENTS%.wxs

rem overwrite files so that modified files show up in the diff
move /Y %EXECUTABLE_FRAGMENTS%.PARAFFIN %EXECUTABLE_FRAGMENTS%.wxs
powershell -Command "(Get-Content %EXECUTABLE_FRAGMENTS%.wxs) | ForEach-Object { $_ -replace 'comp_', 'c' -replace 'file_', 'f' } | Set-Content %EXECUTABLE_FRAGMENTS%.wxs"
move /Y %LIBRARY_FRAGMENTS%.PARAFFIN %LIBRARY_FRAGMENTS%.wxs
powershell -Command "(Get-Content %LIBRARY_FRAGMENTS%.wxs) | ForEach-Object { $_ -replace 'comp_', 'c' -replace 'file_', 'f' } | Set-Content %LIBRARY_FRAGMENTS%.wxs"

goto :eof

:DirectoryNotFound
	echo ########################################################################
	echo ERROR - The expected directory (%2%ExpectedOutputDirectory%) is invalid.
	echo POSSIBLE CAUSE - Did you forget to build your solution?
	echo ########################################################################
	goto :eof

:ParaffinToolNotFound
	echo ###############################################################################################
	echo ERROR - The Paraffin tool was not found.
	echo POSSIBLE CAUSE - NuGet packages have not been properly restored.
	echo ###############################################################################################
	del /F /Q *.wxs
:eof