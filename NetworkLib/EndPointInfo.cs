using System;
using System.Net;
namespace NetworkLib
{
    public struct EndPointInfo:IEquatable<EndPointInfo>
    {
        public EndPointInfo(IPEndPoint remoteEndPoint)
        {
            this.Address=new IPAddress(remoteEndPoint.Address.GetAddressBytes());
            this.Port = remoteEndPoint.Port;
        }

        public IPAddress Address { get; set; }

        public int Port { get; set; }


        public override bool Equals(object obj)
        {
            return this.Equals((EndPointInfo)obj);
        }

        public bool Equals(EndPointInfo other)
        {
            return this.Address.Equals(other.Address) && this.Port==other.Port;
        }

        public override int GetHashCode()
        {
            return this.Address.GetHashCode() ^ this.Port.GetHashCode();
        }

        public static bool operator==(EndPointInfo left,EndPointInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EndPointInfo left, EndPointInfo right)
        {
            return !left.Equals(right);
        }

        public IPEndPoint ToEndPoint()
        {
            return new IPEndPoint(this.Address, this.Port);
        }
    }
}
