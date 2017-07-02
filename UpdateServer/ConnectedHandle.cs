using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UpdateServer
{
    public class ConnectedHandle
    {
        private TcpClient Client { get; set; }
        public NetworkStream SocketStream { get; private set; }
        public Thread ThreadClient { get; set; }
        public List<string> FileNeedUpdate { get; set; } = new List<string>();
        public static List<FilesInformation> FilesInformationsFromClient { get; set; } = new List<FilesInformation>();
        public string Md5FromClient { get; private set; }

        public void StartClient(TcpClient client, string md5FromClient)
        {
            this.Client = client;
            this.SocketStream = Client.GetStream();
            Md5FromClient = md5FromClient;
            ThreadClient = new Thread(DoExchangeData);
            ThreadClient.Start();
        }

        private void DoExchangeData()
        {
            var fileNeedToUpdate = new List<string>();

            if (Md5FromClient == "null")
                fileNeedToUpdate.AddRange(Program.FilesInformations.Select(x => x.PathToFile + ".gz").ToList());
            else
            {
                foreach (var s in Md5FromClient.Split('\n'))
                {
                    FilesInformationsFromClient.Add(new FilesInformation
                    {
                        PathToFile = s.Split(':')[0],
                        Md5HashSum = s.Split(':')[1]
                    });
                }
                var newFiles = Program.FilesInformations.Except(FilesInformationsFromClient).Select(s => s.PathToFile + ".gz").ToList();
                fileNeedToUpdate.AddRange(newFiles);
                fileNeedToUpdate.AddRange(from t in FilesInformationsFromClient
                                          from t1 in Program.FilesInformations
                                          where t.PathToFile == t1.PathToFile
                                          where t.Md5HashSum != t1.Md5HashSum
                                          select t1.PathToFile + ".gz");
            }

            foreach (var v in fileNeedToUpdate)
            {
                Console.WriteLine($"File {v} start to send");
                var clientData = File.ReadAllBytes(v);
                var fileName = Encoding.UTF8.GetBytes(v + " " + clientData.Length + "$");
                WriteAndFlushStream(fileName);

                WriteAndFlushStream(clientData);
                Console.WriteLine($"File {v} was sended");

                while (true)
                {
                    var bytes = new byte[1024];
                    SocketStream.Read(bytes, 0, bytes.Length);
                    var isNext = Encoding.UTF8.GetString(bytes);
                    isNext = isNext.Substring(0, isNext.LastIndexOf("$", StringComparison.Ordinal));
                    if (isNext == "next")
                        break;
                }
            }
            Console.WriteLine($"Client {Client.Client.RemoteEndPoint} was updated");

            Client.Client.Disconnect(true);
            SocketStream.Close();
            ThreadClient.Abort();
        }

        private void WriteAndFlushStream(byte[] sendBytes)
        {
            SocketStream.Write(sendBytes, 0, sendBytes.Length);
            SocketStream.Flush();
        }
    }
}
