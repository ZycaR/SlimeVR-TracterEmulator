namespace SlimeVR_TracterEmulator.TrackerEmulator
{
    public class TrackerDevice
    {
        private const int PACKET_HEARTBEAT = 0;
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

        private NetworkService? networkService;

        public static byte[] MAC(byte deviceId) =>
            [255, deviceId, (byte)(deviceId % 2), 120, 125];


        public bool DiscoverServer(byte deviceId, string firmware)
        {
            if (networkService?.Connected == true) return true;

            networkService = new NetworkService(deviceId);

            Console.WriteLine("Discovering server...");

            // The MAC address doesn't matter, just needs to be consistent
            var mac = MAC(deviceId);
            return networkService.Handshake(mac, firmware);
        }

        private ulong PacketNumber { get; set; }

        public void ConfigureSensor(byte sensor)
        {
            if (networkService?.Connected != true) return;

            using (var packet = new Packet(networkService))
            {
                packet
                    .SendPacketType(PACKET_SENSOR_INFO)
                    .SendPacketNumber(PacketNumber++)
                    .SendByte(sensor) // sensor id
                    .SendByte(1) // sensor state
                    .SendByte(0); // sensor type
            }
        }

        public bool SendRotationQuatSensor(byte sensor, float x, float y, float z, float w)
        {
            if (networkService?.Connected != true) return false;

            using (var packet = new Packet(networkService))
            {
                packet
                    .SendPacketType(PACKET_ROTATION_DATA)
                    .SendPacketNumber(PacketNumber++)
                    .SendByte(sensor) // sensor id
                    .SendByte(1) // DATA_TYPE_NORMAL
                    .SendFloat(x)
                    .SendFloat(y)
                    .SendFloat(z)
                    .SendFloat(w)
                    .SendByte(0); // calibration accuracy(?)
            }

            return true;
        }

        public bool SendRotationQuat(float x, float y, float z, float w) =>
            SendRotationQuatSensor(0, x, y, z, w);
    }
}