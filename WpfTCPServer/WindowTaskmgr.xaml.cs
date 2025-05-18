using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static WpfTCPServer.MainWindow;

namespace WpfTCPServer
{
    /// <summary>
    /// WindowTaskmgr.xaml 的交互逻辑
    /// </summary>
    public partial class WindowTaskmgr : Window
    {
        public ObservableCollection<MainWindow.ProcessItem> ProcessItems { get; set; }
        private MainWindow mainWindow;
        private ClientInfo targetClient;
        public WindowTaskmgr(ObservableCollection<MainWindow.ProcessItem> procs, MainWindow window, ClientInfo client)
        {
            InitializeComponent();
            ProcessItems = procs;
            mainWindow = window;
            targetClient = client;
            this.DataContext = this;
            
        }

        

        private void listView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItem == null && listView.Items.Count > 0)
            {
                listView.SelectedIndex = 0;
            }
        }

        private void MenuItemKill_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is MainWindow.ProcessItem item && targetClient != null)
            {
                try
                {
                    NetworkStream stream = targetClient.TcpClient.GetStream();
                    string pidStr = item.PID.ToString();
                    int cmdType = 4; // KillbyPID
                    mainWindow.sendPackage(stream, cmdType, pidStr);
                    MessageBox.Show($"已发送结束进程 {item.Name} (PID: {pidStr}) 命令", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发送失败: " + ex.Message);
                }
            }

        }

        private void MenuItemRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NetworkStream stream = targetClient?.TcpClient.GetStream();
                mainWindow.sendPackage(stream, 2, "GET TASKS LIST");
            }
            catch (IOException ioex)
            {
                MessageBox.Show(ioex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }

    }
}
