using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SlimeVR_TracterEmulator.TrackerEmulator
{
    internal class NetworkService
    {
        private const int PACKET_HANDSHAKE = 3;

        private Socket Socket { get; set; }
        private IPEndPoint? endpoint;

        private byte[] buffer = new byte[128];
        private byte[] packetBuffer = new byte[1024];
        private int packetBufferHead;
        public bool Connected => endpoint != null;

        public NetworkService(byte deviceId)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                var localEndPoint = new IPEndPoint(IPAddress.Any, 9185 + deviceId);
                Socket.Bind(localEndPoint);

                Socket.EnableBroadcast = true;
                Socket.ReceiveTimeout = 5000;
            }
            catch (SocketException)
            {
                Console.WriteLine($"Failed to bind on port {9185 + deviceId}! You may have trouble reconnecting.");
            }
        }

        public bool Handshake(byte[] mac, string firmware)
        {
            PostHandshake(mac, firmware);

            // Now we need to receive packets and see if we've received back handshake
            endpoint = AckHandshake();

            return Connected;
        }

        private void PostHandshake(byte[] mac, string firmware)
        {
            using (var packet = new Packet(this))
            {
                packet
                    .SendPacketType(PACKET_HANDSHAKE)
                    .SendLong(0) // packet number is always 0
                    .SendInt(0) // board (0 - BOARD_UNKNOWN)
                    .SendInt(0) // imu (0 - ImuID::Unknown)
                    .SendInt(0) // hardware mcu (0 - MCU_UNKNOWN)
                    .SendInt(0)
                    .SendInt(0)
                    .SendInt(0)
                    .SendInt(1) // firmware build number
                    .SendShortString(firmware) // firmware version
                    .SendBytes(mac, mac.Length);
            }
        }

        private IPEndPoint? AckHandshake()
        {
            while (true)
            {
                EndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    if (Socket.ReceiveFrom(packetBuffer, ref sender) <= 0)
                    {
                        Console.WriteLine("recvfrom failed or timed out (is the server running?)");
                        return null;
                    }

                    if (sender is IPEndPoint receivedEndpoint && packetBuffer[0] == PACKET_HANDSHAKE)
                    {
                        string receivedMessage = Encoding.ASCII.GetString(packetBuffer, 1, 12);
                        if (receivedMessage != "Hey OVR =D 5")
                        {
                            Console.WriteLine("response packet did not contain string 'Hey OVR =D 5'");
                            return null;
                        }

                        Console.WriteLine("Handshake successful, connected");
                        return receivedEndpoint;
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("recvfrom failed or timed out (is the server running?)");
                    return null;
                }
            }
        }

        public void BeginPacket()
        {
            packetBufferHead = 0;
        }

        public void EndPacket()
        {
            if (endpoint != null)
            {
                // Send directly to connected host
                Socket.SendTo(packetBuffer, packetBufferHead, SocketFlags.None, endpoint);
            }
            else
            {
                // Broadcast it
                var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 6969);
                Socket.SendTo(packetBuffer, packetBufferHead, SocketFlags.None, broadcastEndpoint);
            }
        }

        public void Write(byte[] buffer, int size)
        {
            Array.Copy(buffer, 0, packetBuffer, packetBufferHead, size);
            packetBufferHead += size;
        }
    }
}
