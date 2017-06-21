using System.IO;
using System.IO.Compression;

namespace UpdateServer.Utilits
{
    public class CompresUtil
    {
        public static void Compres(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var compresFileStream = File.Create(path + ".gz"))
            using (var gZip = new GZipStream(compresFileStream, CompressionMode.Compress))
                fs.CopyTo(gZip);
        }
    }
}
