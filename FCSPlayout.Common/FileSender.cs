using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FCSPlayout.Common
{
    public static class FileSender
    {
        public static void Send(string sourceFilePath, StreamSender.IDataReceiver dataReceiver, int bufferLength = 0)
        {
            using(var source=new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamSender.Send(source, dataReceiver, bufferLength);
            }
        }

        public static void Send(string sourceFilePath, StreamSender.IDataReceiver dataReceiver, CancellationToken cancellationToken, int bufferLength = 0)
        {
            using (var source = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamSender.Send(source, dataReceiver, cancellationToken, bufferLength);
            }
        }

        public static Task SendAsync(string sourceFilePath, StreamSender.IDataReceiver dataReceiver, int bufferLength = 0)
        {
            return Task.Run(() => Send(sourceFilePath, dataReceiver, bufferLength));
        }

        public static Task SendAsync(string sourceFilePath, StreamSender.IDataReceiver dataReceiver, CancellationToken cancellationToken, int bufferLength = 0)
        {
            return Task.Run(() => Send(sourceFilePath, dataReceiver, cancellationToken, bufferLength), cancellationToken);
        }
    }
}