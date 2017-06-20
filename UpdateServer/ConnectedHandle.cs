using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UpdateServer
{
    public class ConnectedHandle
    {
        private TcpClient Client { get; set; }
        public Thread ThreadClient { get; set; }
        public List<string> FileNeedUpdate { get; set; } = new List<string>();
        public static List<FilesInformation> FilesInformationsFromClient { get; set; } = new List<FilesInformation>();

        public ConnectedHandle()
        {

        }

        public void StartClient(TcpClient client)
        {
            this.Client = client;
            ThreadClient = new Thread(DoExchangeData);
            ThreadClient.Start();
        }

        private void DoExchangeData()
        {
            var socketStream = Client.GetStream();
            var bytesFrom = new byte[4096];
            socketStream.Read(bytesFrom, 0, bytesFrom.Length);
            var dataFromClient = Encoding.UTF8.GetString(bytesFrom);
            dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOf("$", StringComparison.Ordinal));
            var md5Files = dataFromClient.Split('$')[1];
            foreach (var s in md5Files.Split('\n'))
            {
                FilesInformationsFromClient.Add(new FilesInformation
                {
                    PathToFile = s.Split(':')[0],
                    Md5HashSum = s.Split(':')[1]
                });
            }
            var list = 
        }
    }
}
