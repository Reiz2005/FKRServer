using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfTCPServer
{
    /// <summary>
    /// cmdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class cmdWindow : Window
    {
        private MainWindow window;
        private ClientInfo clientInfo;
        private DispatcherTimer _syncTimer;
        public cmdWindow(MainWindow mainWindow, ClientInfo client)
        {
            InitializeComponent();
            window = mainWindow;
            clientInfo = client;
            //NetworkStream stream = client.TcpClient.GetStream();
            Dispatcher.Invoke(() =>
            {
                label_title.Content = $"来自 {clientInfo.IpAddress}:{clientInfo.Port} 的CMD回显";
            });

            // 初始化定时器（每500ms检查一次）
            _syncTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _syncTimer.Tick += SyncTextHandler;
            _syncTimer.Start();
        }

        private void SyncTextHandler(object sender, EventArgs e)
        {
            try
            {
                cmdBox.Text = window.servLog_TextBox.Text;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                var result = MessageBox.Show("确定要清空吗？","Warning!",MessageBoxButton.YesNo,MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    cmdBox.Clear();
                }
            });
        }

        private void commandLine_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string cmd_ = commandLine.Text;
                if (!string.IsNullOrEmpty(cmd_))
                {
                    NetworkStream stream = clientInfo.TcpClient.GetStream();
                    window.sendPackage(stream,5,cmd_);
                    Dispatcher.Invoke(() =>
                    {
                        window.log($"[服务器 -> {clientInfo.IpAddress}:{clientInfo.Port}]执行: {cmd_}");
                        commandLine.Clear();
                    });
                }
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string cmd_ = commandLine.Text;
            if (!string.IsNullOrEmpty(cmd_))
            {
                NetworkStream stream = clientInfo.TcpClient.GetStream();
                window.sendPackage(stream, 5, cmd_);
                Dispatcher.Invoke(() =>
                {
                    window.log($"[服务器 -> {clientInfo.IpAddress}:{clientInfo.Port}]执行: {cmd_}");
                    commandLine.Clear();
                });
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            _syncTimer.Stop();
            base.OnClosed(e);
        }
    }
}
