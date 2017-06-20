using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace UpdateServer
{
    internal class Program
    {
        private static string version { get; set; } = String.Empty;

        private static void Main()
        {
            using (StreamReader sr = new StreamReader("version.txt"))
            {
                var s = sr.ReadToEnd().Split(new[] { "Update files:" }, StringSplitOptions.None);
                version = s[0].Substring(0, s[0].Length - 1);
            }



            Console.ReadKey(true);
        }

        private static void tcpConnection()
        {
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"),9656);
            var socketClient = default(TcpClient);
            listener.Start();
            try
            {
                while (true)
                {
                    socketClient = listener.AcceptTcpClient();
                    var socketStream = socketClient.GetStream();
                    var bytesFrom = new byte[4096];
                    socketStream.Read(bytesFrom, 0, bytesFrom.Length);
                    var dataFromClient = Encoding.UTF8.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOf("$", StringComparison.Ordinal));
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void _checkLocalMD5Sum()
        {
            var allfiles = Directory.GetFiles("client", "*.*", SearchOption.AllDirectories);
            var md5HashSum = allfiles.Aggregate(string.Empty, (current, s) => current + s + ":" + getMD5HashFromFile(Path.GetFullPath(s)) + "\n");
            md5HashSum = md5HashSum.Substring(0, md5HashSum.Length - 1);
            using (var fs = new FileStream("md5HashSum.txt", FileMode.CreateNew, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
                sw.WriteLine(md5HashSum);
        }
        private static string getMD5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(fileName))
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}