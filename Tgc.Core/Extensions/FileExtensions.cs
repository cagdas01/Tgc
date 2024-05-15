using System.IO;

namespace Tgc.Core.Extensions
{
    public static class FileExtensions
    {
        //public static string ReadFileContent(string path)
        //{
        //    if (!File.Exists(path))
        //    {
        //        return null;
        //    }
        //    return File.ReadAllText(path);
        //}
        //public static void WriteToFile(string path, string content, string fileName)
        //{
        //    Directory.CreateDirectory(path);
        //    var filePath = Path.Combine(path, fileName);
        //    File.WriteAllText(filePath, content);
        //    //Console.WriteLine($"{fileName} successfully created.");
        //}

        public static void CreateFile(string filePath, string content)
        {
            // Dizin varsa dosya oluştur
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.Write(content);
            }
        }
    }
}