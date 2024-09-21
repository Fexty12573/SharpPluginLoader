
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace BackgroundOptimize;


internal class IniReader(string path)
{
    private readonly string _path = new FileInfo(path).FullName;

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    public string Read(string Key, string Section)
    {
        var RetVal = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", RetVal, 255, _path);
        return RetVal.ToString();
    }
}
