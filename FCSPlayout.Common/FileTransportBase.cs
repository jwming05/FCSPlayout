using System.Threading;
using System.Threading.Tasks;

namespace FCSPlayout.Common
{
    public abstract class FileTransportBase
    {
        protected FileTransportBase()
        {
        }

        public void Receive(string sourceFilePath, int bufferLength = 0)
        {
            using (StreamTransportBase.IDataReceiver dataReceiver = CreateDataReceiver(sourceFilePath))
            {
                FileSender.Send(sourceFilePath, dataReceiver, bufferLength);
            }       
        }

        public void Receive(string sourceFilePath, CancellationToken cancellationToken, int bufferLength = 0)
        {
            using (StreamTransportBase.IDataReceiver dataReceiver = CreateDataReceiver(sourceFilePath))
            {
                FileSender.Send(sourceFilePath, dataReceiver, cancellationToken, bufferLength);
            }
        }

        public Task ReceiveAsync(string sourceFilePath, int bufferLength = 0)
        {
            return Task.Run(() => Receive(sourceFilePath, bufferLength));
        }

        public Task ReceiveAsync(string sourceFilePath, CancellationToken cancellationToken, int bufferLength = 0)
        {
            return Task.Run(() => Receive(sourceFilePath, cancellationToken, bufferLength));
        }

        protected abstract StreamTransportBase.IDataReceiver CreateDataReceiver(string sourceFilePath);
    }
}
