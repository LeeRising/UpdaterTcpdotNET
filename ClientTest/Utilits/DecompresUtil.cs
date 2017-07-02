using System.IO;
using System.IO.Compression;

namespace ClientTest.Utilits
{
    public class DecompresUtil
    {
        public static void Decompress(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var newFileName = path.Remove(path.Length - 3);
                using (var decompresFileStream = File.Create(newFileName))
                using (var gZip = new GZipStream(fs, CompressionMode.Decompress))
                    gZip.CopyTo(decompresFileStream);
            }
        }
    }
}
