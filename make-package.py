import shutil
import subprocess
import argparse
import os

def main(sln_dir, config, tag, skip_build):
    if not os.path.isdir(sln_dir):
        print(f"Error: {sln_dir} is not a directory")
        return False

    if not skip_build:
        # build the solution
        sln = os.path.join(sln_dir, "mhw-cs-plugin-loader.slnx")
        if not os.path.isfile(sln):
            print(f"Error: {sln} not found")
            return False
        print(f"Building solution {sln} with configuration {config}...")
        msbuild = None
        if "ProgramFiles" in os.environ:
            msbuild = f"{os.environ["ProgramFiles"]}\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe"
        if not msbuild or not os.path.isfile(msbuild):
            msbuild = shutil.which("MSBuild.exe")
        if not msbuild:
            print("Error: MSBuild.exe not found")
            return False
        subprocess.run([msbuild, sln, f"/p:Configuration={config}"], check=True)

    # create the release directory
    release_dir = os.path.join(sln_dir, "release-files")
    root_dir_win32 = os.path.join(release_dir, config + "-win32")
    root_dir_linux = os.path.join(release_dir, config + "-linux")
    plugins_dir = os.path.join(root_dir_win32, "nativePC/plugins")
    loader_dir = os.path.join(plugins_dir, "CSharp/Loader")
    tag_dir = os.path.join(release_dir, "tags", tag)

    os.makedirs(loader_dir, exist_ok=True)
    os.makedirs(tag_dir, exist_ok=True)

    boostrapper_src = os.path.join(sln_dir, "SharpPluginLoader.Bootstrapper/bin", config, "net8.0", "SharpPluginLoader.Bootstrapper.dll")
    boostrapper_dst = os.path.join(loader_dir, "SharpPluginLoader.Bootstrapper.dll")

    core_name = "SharpPluginLoader.Core.dll"
    core_src = os.path.join(sln_dir, "SharpPluginLoader.Core/bin", config, "net8.0", core_name)
    core_dst = os.path.join(loader_dir, core_name)

    chunk_name = "Default.bin" if config == "Release" else "Default.Debug.bin"
    chunk_src = os.path.join(sln_dir, "Assets", chunk_name)
    chunk_dst = os.path.join(loader_dir, chunk_name)

    native_src = os.path.join(sln_dir, "x64", config, "mhw-cs-plugin-loader.dll")
    native_winmm_dst = os.path.join(root_dir_win32, "winmm.dll")
    native_ucrtbase_dst = os.path.join(root_dir_linux, "ucrtbase.dll")

    runtimeconfig_src = os.path.join(sln_dir, "mhw-cs-plugin-loader/SharpPluginLoader.runtimeconfig.json")
    runtimeconfig_dst = os.path.join(loader_dir, "SharpPluginLoader.runtimeconfig.json")

    # copy the files for windows (winmm.dll)
    shutil.copyfile(boostrapper_src, boostrapper_dst)
    shutil.copyfile(core_src, core_dst)
    shutil.copyfile(chunk_src, chunk_dst)
    shutil.copyfile(native_src, native_winmm_dst)
    shutil.copyfile(runtimeconfig_src, runtimeconfig_dst)

    # copy the files for linux (ucrtbase.dll)
    shutil.copytree(root_dir_win32, root_dir_linux, dirs_exist_ok=True)
    shutil.copyfile(native_src, native_ucrtbase_dst)
    os.remove(os.path.join(root_dir_linux, "winmm.dll"))

    # create the zip file
    zip_name_win32 = f"SharpPluginLoader-{tag}-{config}-win32" if config == "Debug" else f"SharpPluginLoader-{tag}-win32"
    zip_name_linux = f"SharpPluginLoader-{tag}-{config}-linux" if config == "Debug" else f"SharpPluginLoader-{tag}-linux"
    zip_path_win32 = os.path.join(tag_dir, zip_name_win32)
    zip_path_linux = os.path.join(tag_dir, zip_name_linux)

    shutil.make_archive(zip_path_win32, "zip", root_dir_win32)
    shutil.make_archive(zip_path_linux, "zip", root_dir_linux)

    print(f"Created release: {zip_path_win32}.zip")
    print(f"Created release: {zip_path_linux}.zip")

    return True

def usage(parser: argparse.ArgumentParser):
    parser.print_help()
    exit(1)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Create a release of the plugin loader", add_help=False)
    parser.add_argument("-h", "--help", action="help", help="Show this message")
    parser.add_argument("-s", "--skip-build", help="Skip building the solution", action="store_true")
    parser.add_argument("-c", "--config", help="The configuration to build", default="Release", choices=["Release", "Debug"], type=str.capitalize)
    parser.add_argument("tag", help="The tag to use for the release in the format x.x.x[.x]", nargs="?", default="latest")
    parser.add_argument("sln_dir", help="The directory of the solution", nargs="?", default=".")
    args = parser.parse_args()

    if not main(args.sln_dir, args.config, args.tag, args.skip_build):
        usage(parser)
