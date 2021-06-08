using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpSocket s = new UdpSocket();
            s.Server("127.0.0.1", 27000);

            UdpSocket c = new UdpSocket();
            c.Client("127.0.0.1", 27000);
            c.Send("TEST!");

            Console.ReadKey();
        }
    }
}
