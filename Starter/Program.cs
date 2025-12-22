using System.Diagnostics;
using System.Reflection;

namespace Starter;

internal class Program
{
    static void Main()
    {
        Thread.Sleep(60);
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var name = Path.Combine(dir, "SmartLogViewer.exe");
        if (File.Exists(name))
        {
            Process.Start(name);
        }
    }
}
