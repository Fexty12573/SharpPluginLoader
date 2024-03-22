import shutil
import subprocess
import argparse
import os

def main(sln_dir, config, tag):
    # build the solution

    print(f"Building solution in {sln_dir} with configuration {config}...")
    
    msbuild = f"{os.environ['ProgramFiles']}\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe"
    sln = os.path.join(sln_dir, "mhw-cs-plugin-loader.sln")
    subprocess.run([msbuild, sln, f"/p:Configuration={config}"], check=True)

    # create the release directory
    release_dir = os.path.join(sln_dir, "release-files")
    root_dir = os.path.join(release_dir, config)
    plugins_dir = os.path.join(root_dir, "nativePC/plugins")
    loader_dir = os.path.join(plugins_dir, "CSharp/Loader")
    tag_dir = os.path.join(release_dir, "tags", tag)

    os.makedirs(loader_dir, exist_ok=True)
    os.makedirs(tag_dir, exist_ok=True)

    boostrapper_src = os.path.join(sln_dir, "SharpPluginLoader.Bootstrapper/bin", config, "net8.0", "SharpPluginLoader.Bootstrapper.dll")
    boostrapper_dst = os.path.join(loader_dir, "SharpPluginLoader.Bootstrapper.dll")

    core_name = "SharpPluginLoader.Core.dll" if config == "Release" else "SharpPluginLoader.Core.Debug.dll"
    core_src = os.path.join(sln_dir, "SharpPluginLoader.Core/bin", config, "net8.0", core_name)
    core_dst = os.path.join(loader_dir, core_name)

    chunk_name = "Default.bin" if config == "Release" else "Default.Debug.bin"
    chunk_src = os.path.join(sln_dir, "Assets", chunk_name)
    chunk_dst = os.path.join(loader_dir, chunk_name)

    native_src = os.path.join(sln_dir, "x64", config, "mhw-cs-plugin-loader.dll")
    native_dst = os.path.join(root_dir, "winmm.dll")

    runtimeconfig_src = os.path.join(sln_dir, "mhw-cs-plugin-loader/SharpPluginLoader.runtimeconfig.json")
    runtimeconfig_dst = os.path.join(loader_dir, "SharpPluginLoader.runtimeconfig.json")

    # copy the files
    shutil.copyfile(boostrapper_src, boostrapper_dst)
    shutil.copyfile(core_src, core_dst)
    shutil.copyfile(chunk_src, chunk_dst)
    shutil.copyfile(native_src, native_dst)
    shutil.copyfile(runtimeconfig_src, runtimeconfig_dst)

    # create the zip file
    zip_name = f"SharpPluginLoader-{tag}-{config}" if config == "Debug" else f"SharpPluginLoader-{tag}"
    zip_path = os.path.join(tag_dir, zip_name)
    shutil.make_archive(zip_path, 'zip', root_dir)

    print(f"Created release: {zip_path}.zip")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Create a release of the plugin loader")
    parser.add_argument("sln_dir", help="The directory of the solution")
    parser.add_argument("-c", "--config", help="The configuration to build", default="Release", choices=["Release", "Debug"], type=str.capitalize)
    parser.add_argument("tag", help="The tag to use for the release in the format x.x.x[.x]", default="latest")
    args = parser.parse_args()

    main(args.sln_dir, args.config, args.tag)
