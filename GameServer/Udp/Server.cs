using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udp
{
    public class Server
    {
        Random rand = new Random();
        Queue<byte[]> queue = new Queue<byte[]>();
        UdpClient server;
        private int localPort; // local port
        private bool appClose;
        List<IPEndPoint> clients;
        List<string> bombsCount;

        public delegate void ReceiveHandler(string message);
        public event ReceiveHandler Notify;

        public Server(int localPort)
        {
            try
            {
                this.localPort = localPort;
                appClose = false;
                server = new UdpClient(localPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendData(IPEndPoint client, string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                server.Send(data, data.Length, client);
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
            }
        }

        public void RunServer()
        {
            RunRecieve();
        }

        private void RunRecieve()
        {
            Thread receiveThread = new Thread(new ThreadStart(ReceiveDataConnect));
            receiveThread.Start();
        }

        public void ReceiveDataConnect()
        {
            clients = new List<IPEndPoint>();
            bombsCount = new List<string>();
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                while (!appClose && clients.Count < 2)
                {
                    byte[] data = server.Receive(ref remoteIp); // получаем данные
                    string message = Encoding.Unicode.GetString(data);
                    clients.Add(remoteIp);
                    bombsCount.Add(message);

                    Notify($"{remoteIp.Address}:{remoteIp.Port} - Ready to connect!");
                }
                Notify($"Play!");
                int i = rand.Next(5);
                
                SendData(clients[0], bombsCount[1].ToString() + "|" + i + "|1");
                Notify(i.ToString());
                SendData(clients[1], bombsCount[0].ToString() + "|" + i + "|2");
                Notify(i.ToString());
                SendData(clients[0], "Connect");
                SendData(clients[1], "Connect");
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
            }
        }

        public void Close()
        {
            if (server != null)
                server.Close();
            appClose = true;
        }
    }
}
