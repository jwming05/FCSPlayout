using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FCSPlayout.Common
{
    public abstract class StreamTransportBase
    {
        public interface IDataReceiver : StreamSender.IDataReceiver, IDisposable
        {
        }

        protected StreamTransportBase()
        {
        }

        public void Receive(Stream source, int bufferLength = 0)
        {
            using (IDataReceiver dataReceiver = CreateDataReceiver())
            {
                StreamSender.Send(source, dataReceiver, bufferLength);
            }
        }

        public void Receive(Stream source, CancellationToken cancellationToken, int bufferLength = 0)
        {
            using (IDataReceiver dataReceiver = CreateDataReceiver())
            {
                StreamSender.Send(source, dataReceiver, cancellationToken, bufferLength);
            }
        }

        public Task ReceiveAsync(Stream source, int bufferLength = 0)
        {
            return Task.Run(() => Receive(source, bufferLength));
        }

        public Task ReceiveAsync(Stream source, CancellationToken cancellationToken, int bufferLength = 0)
        {
            return Task.Run(() => Receive(source, cancellationToken, bufferLength));
        }

        protected abstract IDataReceiver CreateDataReceiver();
    }
}
