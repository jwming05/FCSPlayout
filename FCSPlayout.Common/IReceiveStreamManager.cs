using System.IO;

namespace FCSPlayout.Common
{
    public interface IReceiveStreamManager
    {
        // sourceFilePath可以为null。
        Stream CreateReceiveStream(string sourceFilePath);
        void CloseReceiveStream(Stream receiveStream, bool cancelled);
        void PreWrite(byte[] data, int offset, int count);
        void SetTotalLength(long length);
        void PostWrite(byte[] data, int offset, int count);
    }
}
