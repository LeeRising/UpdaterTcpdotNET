using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ClientTest.Utilits
{
    public class Md5Util
    {
        public void _checkLocalMD5Sum(string path)
        {
            var allfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            var md5HashSum = allfiles.Aggregate(string.Empty, (current, s) => current + s + ":" + GetMd5HashFromFile(Path.GetFullPath(s)) + "\n");
            md5HashSum = md5HashSum.Substring(0, md5HashSum.Length - 1);
            using (var fs = new FileStream("md5HashSum.txt", FileMode.CreateNew, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
                sw.WriteLine(md5HashSum);
        }
        private static string GetMd5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(fileName))
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
