using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UpdateServer.Utilits;

namespace UpdateServer
{
    internal class Program
    {
        public static string Version { get; set; } = string.Empty;
        public static string Md5CheckSum { get; set; } = string.Empty;
        public static List<FilesInformation> FilesInformations { get; set; } = new List<FilesInformation>();
        public static bool Isclosing { get; private set; }
        public static string DateTimeNow => DateTime.Now.ToString("[dd-MM-yyyy HH:mm:ss] ");

        private static void Main()
        {
            Console.WriteLine(DateTimeNow + "Server started\n");
            var allfiles = Directory.GetFiles("client", "*.gz", SearchOption.AllDirectories);
            if (allfiles.Length > 0)
                foreach (var s in allfiles)
                    File.Delete(s);

            Console.WriteLine(DateTimeNow + "Check version and get files md5-hash");
            using (var sr = new StreamReader("version.txt"))
                Version = sr.ReadToEnd();
            Md5CheckSum = _checkLocalMD5Sum();

            Console.WriteLine(DateTimeNow + "Start files decomprasion");
            foreach (var s in Md5CheckSum.Split('\n'))
            {
                FilesInformations.Add(new FilesInformation
                {
                    PathToFile = s.Split(':')[0],
                    Md5HashSum = s.Split(':')[1]
                });
                CompresUtil.Compres(s.Split(':')[0]);
            }
            Console.WriteLine(DateTimeNow + "Files was decomprased\n");

            TcpConnection();
        }

        private static void TcpConnection()
        {
            var listener = new TcpListener(IPAddress.Parse("10.154.0.2"), 9656);//10.154.0.2
            listener.Start();
            Console.WriteLine(DateTimeNow + "TcpListener started");
            try
            {
                while (true)
                {
                    var socketClient = listener.AcceptTcpClient();
                    var socketStream = socketClient.GetStream();
                    Console.WriteLine(DateTimeNow + $"Client {socketClient.Client.RemoteEndPoint} is connect");
                    var bytesFrom = new byte[4096];
                    socketStream.Read(bytesFrom,0,bytesFrom.Length);
                    var dataFromClient = Encoding.UTF8.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOf("$", StringComparison.Ordinal));
                    if (dataFromClient.Split('$')[0] != Version)
                        new ConnectedHandle().StartClient(socketClient, dataFromClient.Split('$')[1]);
                    else
                    {
                        var sendBytes = Encoding.UTF8.GetBytes("Already is up to date$");
                        socketStream.Write(sendBytes, 0, sendBytes.Length);
                        socketClient.Client.Disconnect(true);
                        socketStream.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

        #region CloseConsoleHoock
        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    Isclosing = true;
                    break;
                case CtrlTypes.CTRL_C_EVENT:
                    Isclosing = true;
                    break;
                case CtrlTypes.CTRL_BREAK_EVENT:
                    Isclosing = true;
                    break;
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    Isclosing = true;
                    break;

            }
            return true;
        }
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }

    public class FilesInformation
    {
        public string PathToFile { get; set; }
        public string Md5HashSum { get; set; }
    }
}