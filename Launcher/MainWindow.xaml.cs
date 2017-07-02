using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkStream _serverStream;
        private TcpClient _serverSocket;
        protected readonly Thread Thread;
        private byte[] _bytesFromServer;

        public MainWindow()
        {
            InitializeComponent();
            Thread = new Thread(GetServerData);
            TestDownloadBtn.Click += delegate
            {
                try
                {
                    _serverSocket = new TcpClient("35.189.94.197", 9656);
                    _serverStream = _serverSocket.GetStream();
                    var bytes = Encoding.UTF8.GetBytes("11010$null" + "$");
                    _serverStream.Write(bytes, 0, bytes.Length);
                    _serverStream.Flush();
                    Thread.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            };
        }

        private async void GetServerData()
        {
            try
            {
                while (true)
                {
                    _bytesFromServer = new byte[1024];
                    _serverStream.Read(_bytesFromServer, 0, _bytesFromServer.Length);
                    var fileNameAndSize = Encoding.UTF8.GetString(_bytesFromServer);
                    fileNameAndSize = fileNameAndSize.Substring(0, fileNameAndSize.LastIndexOf("$", StringComparison.Ordinal));
                    var fileSize = Convert.ToInt32(fileNameAndSize.Split(' ')[1]);
                    
                    await LogTb.Dispatcher.InvokeAsync(() =>
                    {
                        LogTb.Text += $"{fileNameAndSize.Split(' ')[0]} [{fileNameAndSize.Split(' ')[1]}]\n";
                    });

                    var fullFileBytes = new byte[fileSize];
                    _serverStream.Read(fullFileBytes, 0, fullFileBytes.Length);
                    using (var write = new BinaryWriter(File.Open(fileNameAndSize.Split(' ')[0], FileMode.CreateNew)))
                        write.Write(fullFileBytes);
                    _serverStream.Write(Encoding.UTF8.GetBytes("next$"),0,5);
                    _serverStream.Flush();
                }
            }
            catch (Exception)
            {
                _serverSocket.Client.Disconnect(true);
                _serverStream.Close();
                Thread.Abort();
            }
        }
    }
}
