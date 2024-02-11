using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SharpPluginLoader.Core;

namespace PlayerAnimationViewer;

public class LmtBitMapping
{
    public Dictionary<string, Dictionary<string, List<LmtParamMemberBitMapping>>> Mapping { get; set; } = [];
    
    public List<LmtParamMemberBitMapping> GetBitMapping(string paramName, string paramMemberName)
    {
        if (Mapping.TryGetValue(paramName, out var lmt))
        {
            if (lmt.TryGetValue(paramMemberName, out var param))
            {
                return param;
            }
        }

        return [];
    }

    public static LmtBitMapping LoadFrom(string path)
    {
        var mapping = JsonSerializer
            .Deserialize<Dictionary<string, Dictionary<string, List<LmtParamMemberBitMapping>>>>(
                File.ReadAllText(path), 
                JsonOptions
            );

        if (mapping is null)
        {
            Log.Error("Failed to load LMT bit mapping");
            throw new Exception("Failed to load LMT bit mapping");
        }

        foreach (var param in mapping.Values)
        {
            foreach (var paramMember in param.Values)
            {
                paramMember.Sort(
                    (a, b) => a.Bit.CompareTo(b.Bit)
                );
            }
        }

        return new LmtBitMapping { Mapping = mapping };
    }

    public static void SaveTo(string path, LmtBitMapping mapping)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(mapping.Mapping, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        WriteIndented = true
    };
}

public class LmtParamMemberBitMapping
{
    public required int Bit { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
