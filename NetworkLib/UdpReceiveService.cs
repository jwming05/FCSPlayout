using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkLib
{
    public class UdpReceiveService : IDisposable
    {
        private Task _receiveTask;
        private CancellationTokenSource _cts;
        private UdpClient _udpClient;
        private IPEndPoint _remoteEndPoint;

        public event EventHandler<NetDataReceivedEventArgs> DataReceived;
        public UdpReceiveService(IPEndPoint localEndPoint)
        {
            _udpClient = new UdpClient(localEndPoint);
            _udpClient.EnableBroadcast = true;
            //_udpClient.DontFragment = true;
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public void Start()
        {
            if (_receiveTask == null)
            {
                _cts = new CancellationTokenSource();
                _receiveTask = new Task(ReceiveProc);
                _receiveTask.Start();
            }

        }

        public void Stop()
        {
            if (_receiveTask != null)
            {
                _cts.Cancel();
                _receiveTask.Wait();

                _cts.Dispose();
                _cts = null;
                _receiveTask.Dispose();
                _receiveTask = null;
            }
        }

        private void ReceiveProc()
        {
            while (!_cts.IsCancellationRequested)
            {
                byte[] data = null;
                while (_udpClient.Available > 0)
                {
                    data = _udpClient.Receive(ref _remoteEndPoint);
                    if (data != null && data.Length > 0)
                    {
                        OnDataReceived(data, _remoteEndPoint);
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void OnDataReceived(byte[] data, IPEndPoint remoteEndPoint)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new NetDataReceivedEventArgs(data, remoteEndPoint));
            }
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
