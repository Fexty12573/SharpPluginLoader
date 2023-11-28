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
                var root = new FileSystemFolder("/");
                var assemblies = new FileSystemFolder("Assemblies");
                var resources = new FileSystemFolder("Resources");

                var exeDir = Assembly.GetExecutingAssembly().Location[..^"ChunkBuilder.exe".Length];
                var assetList = args.Length > 0 ? args[0] : exeDir + "/AssetList.txt";
                var assets = File.ReadAllLines(assetList);
                var outputFile = assets.FirstOrDefault(s => s.StartsWith("Out:"), "Out:./Out.bin")[4..];

                foreach (var asset in assets[1..])
                {
                    var file = CreateFile(exeDir + "/" + asset);
                    if (asset.EndsWith(".dll"))
                        assemblies.Add(file);
                    else
                        resources.Add(file);
                }

                root.Add(assemblies);
                root.Add(resources);
                var chunk = new Chunk(root);

                chunk.WriteToFile(exeDir + "/" + outputFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal static FileSystemFile CreateFile(string path)
        {
            return new FileSystemFile(Path.GetFileName(path), File.ReadAllBytes(path));
        }
    }
}
