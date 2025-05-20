using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using static WpfTCPServer.WindowTaskmgr;

namespace WpfTCPServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        private bool isRunning;

        private readonly object _clientLock = new object();
        private readonly Dictionary<string, ClientInfo> _clientMap = new Dictionary<string, ClientInfo>();
        private string[] cmdType_str = { "normal", "test", "getTasksList", "MsgBox", "KillbyPID","cmd","mute","capture" }; //命令类型
        private bool isTaskMgrOpen = false;
        private WindowTaskmgr taskMgrWindow = null;
        private cmdWindow windowCmd = null;
        private DispatcherTimer logSaver = null;
        private int logSaver_interval = 10000;

        public MainWindow()
        {
            InitializeComponent();
            DefaultSettings();
            logSaver = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(logSaver_interval),
            };
            logSaver.Tick += LogSaver_Tick;
            logSaver.Start();
        }

        private void LogSaver_Tick(object sender, EventArgs e)
        {
            try
            {
                string filePath = @"serv" + DateTime.Now.ToString("yyyy-MM-dd")+".log";
                string content = servLog_TextBox.Text;

                // 确保目录存
                File.WriteAllText(filePath, content);

            }
            catch (IOException ex)
            {
                MessageBox.Show("日志保存失败: "+ex.Message,"logSaver");
            }
        }

        private void DefaultSettings()
        {
            servPort_TextBox.Text = "5000";
            logSaver_interval = int.Parse(saveSec_TextBox.Text);
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            String port_str = servPort_TextBox.Text;
            int port = int.Parse(port_str); //默认端口 5000
            if (!isRunning)
            {
                try
                {
                    //创建tcp监听
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();
                    isRunning = true;

                    //启动监听线程
                    listenThread = new Thread(ListenLoop);
                    listenThread.IsBackground = true;
                    listenThread.Start();

                    log($"[服务器] 服务器已在 {port} 端口上启动");

                    BtnStart.Content = "关闭服务器";


                }
                catch(Exception ex)
                {
                    log("[服务器][错误] " + ex.Message);
                }
            }
            else
            {
                stopServ();
            }
            
        }

        private void stopServ()
        {
            isRunning = false;
            tcpListener?.Stop();
            listenThread?.Join(500);
            log("[服务器] 服务器已关闭！");
            BtnStart.Content = "启动服务器";
            lock (_clientLock)
            {
                foreach (var info in _clientMap.Values)
                {
                    info.TcpClient.Close();
                }
                _clientMap.Clear();
                Dispatcher.Invoke(() => clientsList.Items.Clear());
            }
        }
        private void ListenLoop()
        {
            try
            {
                while (isRunning) 
                {
                    //阻塞至客户端连接为止
                    TcpClient client = tcpListener.AcceptTcpClient();
                    IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                    string clientIP = remoteIpEndPoint?.Address.ToString() ?? "Unknown";
                    int clientPort = remoteIpEndPoint?.Port ?? 0;
                    log($"[客户端] {clientIP}:{clientPort}已连接");

                    var info = new ClientInfo
                    {
                        IpAddress = clientIP,
                        Port = clientPort,
                        ConnectedAt = DateTime.Now,
                        TcpClient = client
                    };

                    Dispatcher.Invoke(() =>
                    {
                        lock (_clientLock)
                        {
                            _clientMap[clientIP] = info;
                            clientsList.Items.Add(info);
                        }
                    });

                    Thread clientThread = new Thread(() => HandleClient(info));
                    clientThread.IsBackground=true;
                    clientThread.Start();
                }
            }
            catch (SocketException) { }
            catch (Exception ex)
            {
                log("[服务器][错误] " + ex.Message);
            }
        }
        public void log(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                servLog_TextBox.AppendText(msg + "\n");
                servLog_TextBox.ScrollToEnd();
            });
        }

        //处理每个客户端的线程
        private void HandleClient(ClientInfo info) 
        {
            using (NetworkStream stream = info.TcpClient.GetStream())
            {
                try
                {
                    while (isRunning) 
                    {
                        byte[] headerBuffer = new byte[8]; //存储消息头的缓冲区
                        if (!isReadFully(stream,headerBuffer,8)) break;
                        //头部部分
                        int bodyLength = BitConverter.ToInt32(headerBuffer, 0);
                        int cmdType = BitConverter.ToInt32(headerBuffer, 4);
                        //消息体部分
                        byte[] bodyBuffer = new byte[bodyLength];
                        int totalRead = 0;
                        while (totalRead < bodyLength)
                        {
                            int bytesRead = stream.Read(bodyBuffer, totalRead, bodyLength - totalRead);
                            if (bytesRead == 0) break; //100%丢失说明断开
                            totalRead += bytesRead;
                        }
                        //字符串 收到的消息
                        string received_str;
                        try
                        {
                            //字符串 收到的消息
                            received_str = Encoding.UTF8.GetString(bodyBuffer);
                        }
                        catch (DecoderFallbackException e)
                        {
                            received_str = null;
                            log($"[解码错误] 数据传回来并不能解码成字符串，可能是图片等: {e.Message}");
                        }
                        
                        
                        switch (cmdType)
                        {
                            //命令类型判断
                            /*0 为一般信息
                             * 1 为请求服务端向客户端发送一条测试消息，验证连接
                             * 
                            */
                            case 0:
                                log($"[客户端 {info.IpAddress}:{info.Port}] {received_str}");
                                break;
                            case 1:
                                log($"[客户端 {info.IpAddress}:{info.Port}][{cmdType_str[cmdType]}] {received_str}");
                                //回应内容
                                string response = "1";
                                int ct = 1; //命令类型
                                sendPackage(stream,ct,response);
                                log($"[服务器][->{info.IpAddress}:{info.Port}][{cmdType_str[ct]}] {response}");
                                break;
                            case 2:
                                log($"[客户端 {info.IpAddress}:{info.Port}][{cmdType_str[cmdType]}] 向服务器发送任务列表json");
                                ObservableCollection<ProcessItem> processList = new ObservableCollection<ProcessItem>(JsonConvert.DeserializeObject<List<ProcessItem>>(received_str));
                                if (isTaskMgrOpen == false)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        taskMgrWindow = new WindowTaskmgr(processList, this, info);
                                        isTaskMgrOpen = true;
                                        // 注册窗口关闭事件
                                        taskMgrWindow.Closed += (s, e) =>
                                        {
                                            isTaskMgrOpen = false;
                                            taskMgrWindow = null;
                                        };

                                        taskMgrWindow.Show();
                                    });
                                }
                                else
                                {
                                    MessageBox.Show("已有任务窗口！刷新完成！");
                                    Dispatcher.Invoke(() =>
                                    {
                                        taskMgrWindow.Activate();
                                    });
                                }
                                break;
                            case 5:
                                log($"[客户端 {info.IpAddress}:{info.Port}][CMD回显] {received_str}");
                                break;
                            case 7:
                                log($"[客户端 {info.IpAddress}:{info.Port}][{cmdType_str[cmdType]}] 传回截图");
                                string filePath = $"screenshot-{DateTime.Now:HH-mm-ss}.jpg";
                                File.WriteAllBytes(filePath,bodyBuffer);
                                log("截图已保存！");
                                Process.Start("cmd", $"/c start \"\" \"{System.IO.Path.GetFullPath(filePath)}\"");
                                break;
                            default:
                                log($"[客户端 {info.IpAddress}:{info.Port}] 发送了未知命令 {received_str}");
                                break;
                        }
                        string recvTime = DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss"); //收到消息的时间点
                        log($"{recvTime}");
                    }
                }
                catch (IOException ex)
                {
                    log($"[客户端 {info.IpAddress}:{info.Port}][错误] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");

                }
                finally
                {
                    info.TcpClient.Close();
                    log($"[客户端 {info.IpAddress}:{info.Port}] 断开连接");
                    Dispatcher.Invoke(() =>
                    {
                        lock (_clientLock)
                        {
                            _clientMap.Remove(info.IpAddress);
                            clientsList.Items.Remove(info);
                        }
                    });
                }
            }   

        }

        private void MenuItem_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info)
            {
                lock (_clientLock)
                {
                    try
                    {
                        info.TcpClient.Close();
                        log($"[服务器] 强制断线: {info.IpAddress}:{info.Port}");
                    }
                    catch (Exception ex)
                    {
                        log($"[客户端][错误] {info.IpAddress}:{info.Port}> {ex.Message}");
                    }
                }
            }
        }

        private void MenuItemSend_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                try
                {

                    NetworkStream stream = info.TcpClient.GetStream();
                    //回应内容
                    int ct = 0; //命令类型
                    sendMessageDialog dialog = new sendMessageDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        string response = dialog.text_str;
                        if (!string.IsNullOrEmpty(response))
                        {
                            //MessageBox.Show(str, "info");
                            if (dialog.msgbox) ct = 3;
                            sendPackage(stream, ct, response);
                            log($"[服务器][->{info.IpAddress}:{info.Port}][{cmdType_str[ct]}] {response}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log("[服务器][错误] " + ex.Message);
                }
            }
               
                
        }

        private void MenuItemTaskmgr_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                try
                {
                    NetworkStream stream = info.TcpClient.GetStream();
                    int ct = 2;
                    sendPackage(stream, ct, "GET TASKS LIST");
                    log($"[服务器][->{info.IpAddress}:{info.Port}]已经发送获取任务列表请求");
                }
                catch (IOException ioex)
                {
                    log("[服务器][IO错误] " + ioex.Message);
                }

            }
        }

        public class ProcessItem
        {
            public string Name { get; set; }
            public int PID { get; set; }
            public string Module { get; set; }
        }
        bool isReadFully(NetworkStream stream, byte[] buffer, int size)//判断是否读取到了指定字节数
        {
            int offset = 0;
            while (size > 0)
            {
                try
                {
                    int read = stream.Read(buffer, offset, size);
                    if (read <= 0)
                    {
                        return false; // 对端关闭连接
                    }
                    offset += read;
                    size -= read;
                }
                catch (IOException ioex)
                {
                    log("[错误] IOException: " + ioex.Message);
                    return false; // 被 Cancel 或断开了
                }
                catch (Exception ex)
                {
                    log("[错误] 异常: " + ex.Message);
                    return false;
                }
            }
            return true;
        }
        public void sendPackage(NetworkStream stream, int cmdType, string body_str)
        {
            byte[] bytesBody = Encoding.UTF8.GetBytes(body_str);
            byte[] bytesOfBodyLen = BitConverter.GetBytes(bytesBody.Length);
            byte[] bytesOfCmdtype = BitConverter.GetBytes(cmdType);

            stream.Write(bytesOfBodyLen, 0, 4);
            stream.Write(bytesOfCmdtype, 0, 4);
            stream.Write(bytesBody,0,bytesBody.Length);
        }

        private void MenuItemCmd_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                Dispatcher.Invoke(() =>
                {
                    windowCmd = new cmdWindow(this, info);
                    windowCmd.Closed += (s, e_) =>
                    {
                        windowCmd = null;
                    };
                    windowCmd.Show();
                });
            }
        }

        private void MenuItemShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                var result = MessageBox.Show("确定给他关机了吗？","shutdown",MessageBoxButton.YesNo,MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    NetworkStream stream = info.TcpClient.GetStream();
                    sendPackage(stream, 5, "shutdown -s -t 0");
                }
            }
        }

        private void MenuItemRestart_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                var result = MessageBox.Show("确定给他重启了吗？", "shutdown", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    NetworkStream stream = info.TcpClient.GetStream();
                    sendPackage(stream, 5, "shutdown -r -t 0");
                }
            }
        }

        private void MenuItemMute_Click(object sender, RoutedEventArgs e)
        {
            if (clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                NetworkStream stream = info.TcpClient.GetStream();
                sendPackage(stream, 6, "MUTE");
            }
        }

        private void MenuItemCapture_Click(object sender, RoutedEventArgs e)
        {
            if(clientsList.SelectedItem is ClientInfo info && isRunning)
            {
                NetworkStream stream = info.TcpClient.GetStream();
                sendPackage(stream,7,"cap");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure to exit?","Quit",MessageBoxButton.OKCancel,MessageBoxImage.Warning);
            if(result == MessageBoxResult.OK)
            {
                logSaver.Stop();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
