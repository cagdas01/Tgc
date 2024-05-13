using System;
using System.IO;
namespace Tgc.Core.Extensions;
public static class FileExtensions
{
    public static string ReadFileContent(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        return File.ReadAllText(path);
    }

    public static void WriteToFile(string path, string content, string fileName)
    {
        File.WriteAllText(path, content);
        Console.WriteLine($"{fileName} successfully created.");
    }
}
