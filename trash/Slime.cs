using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SlimeVR_TracterEmulator.trash
{
    public static class Slime
    {
        private const int PACKET_HEARTBEAT = 0;
        private const int PACKET_HANDSHAKE = 3;
        private const int PACKET_ACCEL = 4;
        private const int PACKET_CONFIG = 8;
        private const int PACKET_PING_PONG = 10;
        private const int PACKET_SERIAL = 11;
        private const int PACKET_BATTERY_LEVEL = 12;
        private const int PACKET_TAP = 13;
        private const int PACKET_ERROR = 14;
        private const int PACKET_SENSOR_INFO = 15;
        private const int PACKET_ROTATION_DATA = 17;
        private const int PACKET_MAGNETOMETER_ACCURACY = 18;
        private const int PACKET_SIGNAL_STRENGTH = 19;
        private const int PACKET_TEMPERATURE = 20;
        private const int PACKET_FEATURE_FLAGS = 22;

        private static void SendLongString(string str)
        {
            //int size = str.Length;
            //SendInt((uint)size);
            //SendBytes(Encoding.ASCII.GetBytes(str), size);
        }

        public static void SendHeartbeat()
        {
            //if (!slimeNetworkState.Connected) return;

            //BeginPacket();
            //SendPacketType(PACKET_HEARTBEAT);
            //SendPacketNumber();
            //EndPacket();
        }

    }
}