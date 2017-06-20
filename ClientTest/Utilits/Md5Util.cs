using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ClientTest.Utilits
{
    public class Md5Util
    {
        public string _checkLocalMD5Sum(string path)
        {
            var allfiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            var md5Sum = allfiles.Aggregate(string.Empty, (current, s) => current + s + ":" + GetMd5HashFromFile(Path.GetFullPath(s)) + "\n");
            return md5Sum.Substring(0, md5Sum.Length - 1);
        }
        private static string GetMd5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(fileName))
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
