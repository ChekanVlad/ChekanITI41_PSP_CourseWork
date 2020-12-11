using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Udp
{
    public class Client
    {
        private string remoteAddress; // the addres to which we connect
        private int remotePort; // the port to which we connect
        private int localPort; // local port
        bool appQuit;
        UdpClient client;
        int bomb1, bomb2;

        public delegate void ReceiveHandler(string message);
        public event ReceiveHandler Notify;
        public Client(string ip, int remotePort, int localPort, int bomb1, int bomb2)
        {
            try
            {
                this.localPort = localPort;
                remoteAddress = ip;
                this.remotePort = remotePort;
                appQuit = false;
                client = new UdpClient(localPort);
                this.bomb1 = bomb1;
                this.bomb2 = bomb2;
                RunConnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RunConnect()
        {
            Thread receiveThread = new Thread(new ThreadStart(Connect));
            receiveThread.Start();
        }

        public void Connect()
        {
            Thread.Sleep(100);
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(bomb1 + "|" + bomb2);
                client.Send(data, data.Length, remoteAddress, remotePort);
                ReceiveData();
                ReceiveData();
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
            }
        }

        public void ReceiveData()
        {
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                byte[] data = client.Receive(ref remoteIp); // получаем данные
                string message = Encoding.Unicode.GetString(data);

                Notify(message);
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
            }
        }

        public void CloseClient()
        {
            if (client != null)
                client.Close();
            appQuit = true;
        }

        public void ClearNotify()
        {
            Notify = null;
        }
    }
}
