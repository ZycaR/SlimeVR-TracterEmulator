using System.Net;
using System.Text;

namespace SlimeVR_TracterEmulator.TrackerEmulator
{
    internal class Packet : IDisposable
    {
        private bool _disposed = false;
        private readonly NetworkService _service;

        public Packet(NetworkService service)
        {
            _service = service;
            _service.BeginPacket();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _service.EndPacket();
                }
                _disposed = true;
            }
        }

        ~Packet()
        {
            Dispose(false);
        }

        public Packet SendByte(byte value)
        {
            _service.Write([value], 1);
            return this;
        }

        public Packet SendShort(ushort value)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
            _service.Write(buffer, buffer.Length);
            return this;
        }

        public Packet SendFloat(float value)
        {
            var buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer);  // Reverse for network byte order
            _service.Write(buffer, buffer.Length);
            return this;
        }

        public Packet SendInt(uint value)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)value));
            _service.Write(buffer, buffer.Length);
            return this;
        }

        public Packet SendLong(ulong value)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)value));
            _service.Write(buffer, buffer.Length);
            return this;
        }

        public Packet SendBytes(byte[] buffer, int length)
        {
            _service.Write(buffer, length);
            return this;
        }

        public Packet SendShortString(string str)
        {
            byte size = (byte)str.Length;
            SendByte(size);
            SendBytes(Encoding.ASCII.GetBytes(str), size);
            return this;
        }

        public Packet SendPacketType(byte type) => this
            .SendByte(0)
            .SendByte(0)
            .SendByte(0)
            .SendByte(type);

        public Packet SendPacketNumber(ulong number) => SendLong(number);
    }
}