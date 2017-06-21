using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace UpdateServer
{
    internal class Program
    {
        public static string Version { get; set; } = string.Empty;
        public static string Md5CheckSum { get; set; } = string.Empty;
        public static List<FilesInformation> FilesInformations { get; set; } = new List<FilesInformation>();
        private static void Main()
        {
            using (var sr = new StreamReader("version.txt"))
                Version = sr.ReadToEnd();
            Md5CheckSum = _checkLocalMD5Sum();
            //Console.Write(Md5CheckSum);
            foreach (var s in Md5CheckSum.Split('\n'))
            {
                FilesInformations.Add(new FilesInformation
                {
                    PathToFile = s.Split(':')[0],
                    Md5HashSum = s.Split(':')[1]
                });
            }

            TcpConnection();

            Console.ReadKey(true);
        }

        private static void TcpConnection()
        {
            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9656);
            listener.Start();
            try
            {
                while (true)
                {
                    var socketClient = listener.AcceptTcpClient();
                    var socketStream = socketClient.GetStream();
                    var bytesFrom = new byte[4096];
                    socketStream.Read(bytesFrom, 0, bytesFrom.Length);
                    var dataFromClient = Encoding.UTF8.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOf("$", StringComparison.Ordinal));
                    if (dataFromClient.Split('$')[0] != Version)
                        new ConnectedHandle().StartClient(socketClient);
                    else
                    {
                        var sendBytes = Encoding.UTF8.GetBytes("Already is up to date");
                        socketStream.Write(sendBytes, 0, sendBytes.Length);
                        socketStream.Flush();
                        socketClient.Client.Disconnect(true);
                        socketStream.Close();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static string _checkLocalMD5Sum()
        {
            var allfiles = Directory.GetFiles("client", "*.*", SearchOption.AllDirectories);
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

    public class FilesInformation
    {
        public string PathToFile { get; set; }
        public string Md5HashSum { get; set; }
    }
}