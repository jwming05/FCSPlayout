using System;

namespace NetworkLib
{
    public class MessageSegment
    {
        public static MessageSegment Create(byte[] data)
        {
            if (data.Length < 32) return null;

            try
            {
                return new MessageSegment(data);
            }
            catch
            {
                return null;
            }
        }

        private MessageSegment(byte[] data)
        {
            var guidBuffer = new byte[16];

            Array.Copy(data, 0, guidBuffer, 0, 16);
            this.MessageId = new Guid(guidBuffer);

            this.SegmentNumber = BitConverter.ToInt32(data,16);
            this.TotalLength= BitConverter.ToInt32(data, 20);
            this.Offset= BitConverter.ToInt32(data, 24);
            this.Length= BitConverter.ToInt32(data, 28);

            if (data.Length != this.Length + 32)
            {
                throw new ArgumentException();
            }
            this.Data = new byte[this.Length];
            Array.Copy(data, 32, this.Data, 0, this.Length);
        }

        public Guid MessageId { get; private set; }
        public int SegmentNumber { get; private set; }
        public int TotalLength { get; private set; }

        public int Offset { get; private set; }
        public int Length { get; private set; }

        public byte[] Data { get; private set; }
    }
}
