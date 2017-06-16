using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FCSPlayout.Common
{
    public static class StreamSender
    {
        private const int DefaultBufferLength = 4096;
        public interface IDataReceiver
        {
            void Receive(byte[] data, int offset, int count);

            void Cancel();
            void SetTotalLength(long length);
        }

        public static void Send(Stream source, IDataReceiver dataReceiver, int bufferLength = 0)
        {
            byte[] buffer = CreateBuffer(bufferLength);
            if (source.CanSeek)
            {
                dataReceiver.SetTotalLength(source.Length);
            }
            
            int count = source.Read(buffer, 0, buffer.Length);
            while (count > 0)
            {
                dataReceiver.Receive(buffer, 0, count);
                count = source.Read(buffer, 0, buffer.Length);
            }
        }



        public static void Send(Stream source, IDataReceiver dataReceiver, CancellationToken cancellationToken, int bufferLength = 0)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                dataReceiver.Cancel();
                return;
            }

            byte[] buffer = CreateBuffer(bufferLength);
            if (source.CanSeek)
            {
                dataReceiver.SetTotalLength(source.Length);
            }
            
            int count = source.Read(buffer, 0, buffer.Length);
            while (count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    dataReceiver.Cancel();
                    break;
                }
                else
                {
                    dataReceiver.Receive(buffer, 0, count);
                    count = source.Read(buffer, 0, buffer.Length);
                }
            }
        }

        public static Task SendAsync(Stream source, IDataReceiver dataReceiver, int bufferLength = 0)
        {
            return Task.Run(() => Send(source, dataReceiver, bufferLength));
        }

        public static Task SendAsync(Stream source, IDataReceiver dataReceiver, CancellationToken cancellationToken, int bufferLength = 0)
        {
            return Task.Run(() => Send(source, dataReceiver, cancellationToken, bufferLength), cancellationToken);
        }

        private static byte[] CreateBuffer(int bufferLength)
        {
            return new byte[bufferLength == 0 ? DefaultBufferLength : bufferLength];
        }
    }
}
