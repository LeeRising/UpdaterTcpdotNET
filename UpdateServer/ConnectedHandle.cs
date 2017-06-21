using System;
using System.Collections.Generic;
using System.IO;
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
            var fileNeedToUpdate = new List<string>();
            foreach (var t in FilesInformationsFromClient)
            {
                foreach (var t1 in Program.FilesInformations)
                {
                    if(t.PathToFile == t1.PathToFile)
                        if (t.Md5HashSum != t1.Md5HashSum)
                            fileNeedToUpdate.Add(t1.PathToFile);
                        else
                        {
                            //Ignore
                        }
                    else
                        fileNeedToUpdate.Add(t1.PathToFile);
                }
            }
            foreach (var v in fileNeedToUpdate)
            {
                using (var fs = new FileStream(v, FileMode.Open, FileAccess.Read))
                {
                    var noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fs.Length) / Convert.ToDouble(BufferSize)));
                    //progressBar1.Maximum = NoOfPackets;
                    var TotalLength = (int)fs.Length;
                    var counter = 0;
                    for (var i = 0; i < noOfPackets; i++)
                    {
                        int CurrentPacketLength;
                        if (TotalLength > BufferSize)
                        {
                            CurrentPacketLength = BufferSize;
                            TotalLength = TotalLength - CurrentPacketLength;
                        }
                        else
                            CurrentPacketLength = TotalLength;
                        var sendingBuffer = new byte[CurrentPacketLength];
                        fs.Read(sendingBuffer, 0, CurrentPacketLength);
                        socketStream.Write(sendingBuffer, 0, sendingBuffer.Length);
                        //if (progressBar1.Value >= progressBar1.Maximum)
                        //    progressBar1.Value = progressBar1.Minimum;
                        //progressBar1.PerformStep();
                    }
                }
            }

            Client.Client.Disconnect(true);
            socketStream.Close();
        }
    }
}
