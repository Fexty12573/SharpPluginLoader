@echo off

set /p MHW_DIR="Enter the path to your MHW directory: "
set LOADER_DIR="%MHW_DIR%\nativePC\plugins\CSharp\Loader"
set DIR="%~dp0"

set /p USE_DEBUG="Do you want to use the debug version of the loader? (y/n): "

:: No input, default to no
if not defined USE_DEBUG set USE_DEBUG=n

if not exist %LOADER_DIR% (
    :: Create the Loader directory
    mkdir %LOADER_DIR%
)

if /i %USE_DEBUG% == y (
    set NATIVE_DLL=x64\Debug\mhw-cs-plugin-loader.dll
) else (
    set NATIVE_DLL=x64\Release\mhw-cs-plugin-loader.dll
)

:: Create the symbolic links
:: - cimgui.debug.dll -> Assets/DebugAssets/cimgui.debug.dll
:: - cimgui.dll -> Assets/ReleaseAssets/cimgui.dll
:: - Default.bin -> Assets/Default.bin
:: - Default.Debug.bin -> Assets/Default.Debug.bin
:: - SharpPluginLoader.Bootstrapper.Debug.dll -> SharpPluginLoader.Boostrapper/bin/Debug/net8.0/SharpPluginLoader.Bootstrapper.Debug.dll
:: - SharpPluginLoader.Bootstrapper.dll -> SharpPluginLoader.Boostrapper/bin/Release/net8.0/SharpPluginLoader.Bootstrapper.dll
:: - SharpPluginLoader.Core.Debug.dll -> SharpPluginLoader.Core/bin/Debug/net8.0/SharpPluginLoader.Core.Debug.dll
:: - SharpPluginLoader.Core.dll -> SharpPluginLoader.Core/bin/Release/net8.0/SharpPluginLoader.Core.dll

set /p DELETE_EXISTING="Do you want to delete existing symbolic links? (y/n): "
if not defined DELETE_EXISTING set DELETE_EXISTING=n

if /i %DELETE_EXISTING% == y (
    del %LOADER_DIR%\cimgui.debug.dll
    del %LOADER_DIR%\cimgui.dll
    del %LOADER_DIR%\Default.bin
    del %LOADER_DIR%\Default.Debug.bin
    del %LOADER_DIR%\SharpPluginLoader.Bootstrapper.Debug.dll
    del %LOADER_DIR%\SharpPluginLoader.Bootstrapper.dll
    del %LOADER_DIR%\SharpPluginLoader.Core.Debug.dll
    del %LOADER_DIR%\SharpPluginLoader.Core.dll
    del "%MHW_DIR%\winmm.dll"
)

mklink %LOADER_DIR%\cimgui.debug.dll %DIR%Assets\DebugAssets\cimgui.debug.dll
mklink %LOADER_DIR%\cimgui.dll %DIR%Assets\ReleaseAssets\cimgui.dll
mklink %LOADER_DIR%\Default.bin %DIR%Assets\Default.bin
mklink %LOADER_DIR%\Default.Debug.bin %DIR%Assets\Default.Debug.bin
mklink %LOADER_DIR%\SharpPluginLoader.Bootstrapper.Debug.dll %DIR%SharpPluginLoader.Bootstrapper\bin\Debug\net8.0\SharpPluginLoader.Bootstrapper.dll
mklink %LOADER_DIR%\SharpPluginLoader.Bootstrapper.dll %DIR%SharpPluginLoader.Bootstrapper\bin\Release\net8.0\SharpPluginLoader.Bootstrapper.dll
mklink %LOADER_DIR%\SharpPluginLoader.Core.Debug.dll %DIR%SharpPluginLoader.Core\bin\Debug\net8.0\SharpPluginLoader.Core.dll
mklink %LOADER_DIR%\SharpPluginLoader.Core.dll %DIR%SharpPluginLoader.Core\bin\Release\net8.0\SharpPluginLoader.Core.dll

mklink "%MHW_DIR%\winmm.dll" %DIR%%NATIVE_DLL%

echo Done!
pause
