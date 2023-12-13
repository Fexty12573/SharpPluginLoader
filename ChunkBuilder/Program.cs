using System;
using System.Reflection;
using System.Text;

namespace ChunkBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var envVars = new Dictionary<string, string>();
                if (args.Length > 1)
                {
                    for (var i = 1; i < args.Length; i++)
                    {
                        Console.WriteLine(args[i]);
                        var arg = args[i];
                        var split = arg.Split('=');
                        envVars.Add(split[0], split[1].Trim(['\'', '"']));
                    }
                }

                var root = new FileSystemFolder("/");
                var assemblies = new FileSystemFolder("Assemblies");
                var resources = new FileSystemFolder("Resources");
                var nativeLibs = new FileSystemFolder("NativeLibraries");

                var exeDir = Assembly.GetExecutingAssembly().Location[..^"ChunkBuilder.exe".Length];
                var assetList = args.Length > 0 ? args[0] : exeDir + "/AssetList.txt";
                var assets = File.ReadAllLines(assetList);
                var outputFile = assets.FirstOrDefault(s => s.StartsWith("Out:"), "Out:./Out.bin")[4..];

                foreach (var asset in assets[1..])
                {
                    var processedAsset = ProcessString(asset, envVars);

                    var native = processedAsset.StartsWith("n:");
                    processedAsset = processedAsset[(native ? 2 : 0)..];

                    var file = Path.IsPathFullyQualified(processedAsset)
                        ? CreateFile(processedAsset)
                        : CreateFile(exeDir + "/" + processedAsset);

                    if (processedAsset.EndsWith(".dll"))
                    {
                        if (native)
                            nativeLibs.Add(file);
                        else
                            assemblies.Add(file);
                    }
                    else
                    {
                        resources.Add(file);
                    }
                }

                root.Add(assemblies);
                root.Add(resources);
                root.Add(nativeLibs);
                var chunk = new Chunk(root);

                chunk.WriteToFile(exeDir + "/" + outputFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string ProcessString(string str, Dictionary<string, string> env)
        {
            if (env.Count == 0)
                return str;

            var sb = new StringBuilder();
            var i = 0;
            while (i < str.Length)
            {
                if (str[i] == '$' && i < str.Length - 1 && str[i + 1] == '(')
                {
                    var j = i + 1;
                    while (j < str.Length && str[j] != ')')
                        j++;
                    var varName = str[(i + 2)..j];
                    if (env.TryGetValue(varName, out var value))
                        sb.Append(value);
                    i = j + 1;
                }
                else
                {
                    sb.Append(str[i]);
                    i++;
                }
            }

            return sb.ToString();
        }

        internal static FileSystemFile CreateFile(string path)
        {
            return new FileSystemFile(Path.GetFileName(path), File.ReadAllBytes(path));
        }
    }
}
