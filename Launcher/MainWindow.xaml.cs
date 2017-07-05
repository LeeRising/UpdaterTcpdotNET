using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Thread _thread;
        private byte[] _bytesFromServer;
        private string _fileNameAndSize;
        private const int BlockSize = 1024;

        public MainWindow()
        {
            InitializeComponent();
            TestDownloadBtn.Click += delegate
            {
                _thread = new Thread(GetServerData);
                try
                {
                    _serverSocket = new TcpClient("35.189.94.197", 9656); //35.189.94.197
                    _serverStream = _serverSocket.GetStream();
                    var bytes = Encoding.UTF8.GetBytes("11010$null" + "$");
                    _serverStream.Write(bytes, 0, bytes.Length);
                    _thread.Start();
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
                    _bytesFromServer = new byte[4096];
                    _serverStream.Read(_bytesFromServer, 0, _bytesFromServer.Length);
                    _fileNameAndSize = Encoding.UTF8.GetString(_bytesFromServer);
                    _fileNameAndSize = _fileNameAndSize.Substring(0, _fileNameAndSize.LastIndexOf("$", StringComparison.Ordinal));
                    var fileSize = Convert.ToInt32(_fileNameAndSize.Split(' ')[1]);
                    await LogTb.Dispatcher.InvokeAsync(() =>{LogTb.Text += $"{_fileNameAndSize.Split(' ')[0]} [{fileSize}]\n";});

                    var fullFileBytes = new byte[fileSize];
                    int i = _serverStream.Read(fullFileBytes, 0, fullFileBytes.Length);//12780
                    await LogTb.Dispatcher.InvokeAsync(() => { LogTb.Text += $"{i}\n"; });
                    using (var write = new BinaryWriter(File.Open(_fileNameAndSize.Split(' ')[0], FileMode.OpenOrCreate)))
                        write.Write(fullFileBytes);
                    var nextByte = Encoding.UTF8.GetBytes("next$");
                    _serverStream.Write(nextByte, 0, nextByte.Length);
                }
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.ToString());
                Thread.Sleep(1);
                _serverSocket.Client.Disconnect(true);
                _serverStream.Close();
                _thread.Abort();
            }
        }
    }
}
