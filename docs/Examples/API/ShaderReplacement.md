# Shader Replacement

By implementing `OnCreateShader()`, you can intercept and replace a shader at the point it's loaded.

An example of this process could go like this:
1. Open Monster Hunter World in [RenderDoc](https://renderdoc.org/).
2. Make a capture and find the shader you want to edit.
3. Click View to inspect it's bytecode and get it's hash, click Save to write it to a file.
4. Disassemble the shader with `cmd_Decompiler.exe` from [3Dmigoto](https://github.com/bo3b/3Dmigoto).
```cmd
.\cmd_Decompiler.exe -d target_shader.dxbc
```
5. Edit the DXBC bytecode in `target_shader.asm`.
6. Reassemble the shader.
```cmd
.\cmd_Decompiler.exe -a target_shader.asm
```
7. Use `target_shader.shdr` like the example below.

```cs
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Rendering;

public class Plugin : IPlugin
{
    public unsafe void OnCreateShader(ShaderInfo* info)
    {
        string hash = new string(info->DxbcHash);
        if (hash == "ca33bab6-2119df84-41786a69-404471cd")
        {
            if (info->Replacement.Source != null)
            {
                Log.Warn("Color grading shader already has a replacement.");
                return;
            }
            string path = "nativePC/plugins/CSharp/Shaders/color_grading.shdr";
            if (!File.Exists(path))
            {
                Log.Warn($"File not found: {path}.");
                return;
            }
            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to read file: {path} ({e.Message}).");
                return;
            }
            // Can also use ShaderSourceType.HLSL, supply text and it will be compiled by D3DCompile().
            info->Replacement.Set(bytes, ShaderSourceType.Binary);
            Log.Info("Color grading shader replaced.");
        }
    }
}
```

You can also use 3Dmigoto to decompile a shader to HLSL and try to get that running in RenderDoc to quickly view your changes.
```cmd
.\cmd_Decompiler.exe -D target_shader.dxbc
```
This is a incomplete workflow, but it's still worth a try as it can be very useful. You'll likely run into bugs with both RenderDoc and 3Dmigoto.
