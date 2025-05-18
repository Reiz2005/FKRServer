using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfTCPServer
{
    public class ClientInfo
    {
        public string IpAddress { get; set; }
        public DateTime ConnectedAt { get; set; }
        public int Port { get; set; }
        public TcpClient TcpClient { get; set; }
        public override string ToString()
        {
            return $"{IpAddress}:{Port} (连接时间: {ConnectedAt:HH:mm:ss})";
        }
    }
}
