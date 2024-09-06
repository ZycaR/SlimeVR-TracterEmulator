using CommandLine;

class Options
{
    [Option("deviceid", Required = false, HelpText = "ID of the device.")]
    public byte DeviceId { get; set; } = 0;

    [Option("sensors", Required = false, HelpText = "Number of sensors.")]
    public int Sensors { get; set; } = 1;

    [Option("rotation", Required = false, HelpText = "Axis of rotation (X, Y, Z).")]
    public string Rotation { get; set; } = String.Empty;
}
