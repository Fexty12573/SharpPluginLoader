:: Invoke as post-build.bat <solution-dir> <Debug|Release>

@echo off

if "%1" == "" (
    echo Usage: post-build.bat ^<solution-dir^> ^<Debug^|Release^>
    exit /b 1
)
if "%2" == "" (
    echo Usage: post-build.bat ^<solution-dir^> ^<Debug^|Release^>
    exit /b 1
)

set SOLUTION_DIR=%1

if "%2" == "Debug" (
    set CONFIGURATION=Debug
) else if "%2" == "Release" (
    set CONFIGURATION=Release
)

:: Copy AddressRecords.json
copy /y "%SOLUTION_DIR%\SharpPluginLoader.Core\AddressRecords.json" "%SOLUTION_DIR%\ChunkBuilder\bin\Release\net8.0\Data\AddressRecords.json"
copy /y "%SOLUTION_DIR%\SharpPluginLoader.Core\AddressRecords.json" "%SOLUTION_DIR%\ChunkBuilder\bin\Release\net8.0\Data.Debug\AddressRecords.json"

:: Invoke ChunkBuilder
call "%SOLUTION_DIR%\ChunkBuilder\bin\Release\net8.0\ChunkBuilder.exe" "%SOLUTION_DIR%\ChunkBuilder\AssetList.%CONFIGURATION%.txt" ShaderDir="%SOLUTION_DIR%\mhw-cs-plugin-loader\Shaders\"
