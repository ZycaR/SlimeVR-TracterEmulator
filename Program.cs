using CommandLine;
using SlimeVR_TracterEmulator.TrackerEmulator;
using System.Diagnostics;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<Options>(args)
            .WithNotParsed((errs) =>
            {
                Console.WriteLine("There were errors parsing the arguments:");
                errs.ToList().ForEach(Console.WriteLine);
                Console.WriteLine();

                var assembly = Assembly.GetExecutingAssembly().GetName().Name;
                Console.WriteLine($"Usage: {assembly} -- --deviceid <number> --sensors <number> --rotation <X|Y|Z>");
            })
            .WithParsed(opts =>
            {
                opts.Sensors = Math.Max(1, opts.Sensors); // Ensure at least one sensor
                opts.Sensors = Math.Min(16, opts.Sensors); // Ensure no more than 16 sensors

                // Use the parsed arguments
                Console.WriteLine($"Device ID: {opts.DeviceId}");
                Console.WriteLine($"Number of sensors: {opts.Sensors}");
                Console.WriteLine($"Rotation axis: {opts.Rotation.ToUpper()}");
                var mac = string.Join(".", TrackerDevice.MAC(opts.DeviceId));
                Console.WriteLine($"MAC Address: {mac}");
                Console.WriteLine();

                Emulate(opts);
            });
    }

    static void Emulate(Options opts)
    {
        var tracker = new TrackerDevice();

        var firmware = string.Join("-", ["Tracker", "Emulator", opts.Rotation], 0, string.IsNullOrWhiteSpace(opts.Rotation) ? 2 : 3);

        // Attempt to discover the server until successful
        while (!tracker.DiscoverServer(opts.DeviceId, firmware))
        {
            // Optionally, you could add a short delay here to prevent tight looping
            Thread.Sleep(100);
        }

        for (byte i = 0; i < opts.Sensors - 1; i++)
        {
            tracker.ConfigureSensor(i);
        }

        // Set up the rotation period (10 seconds for a full rotation)
        const double rotationPeriod = 10.0; // seconds
        var rotationAxis = opts.Rotation.ToUpperInvariant();

        Console.WriteLine(@"------------------------------------------------------------
Press X, Y, or Z to change the rotation axis.
Press SPACE to stop rotation.
Press any key to exit.
------------------------------------------------------------");


        // Continuously send rotation quaternion
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                rotationAxis = key switch
                {
                    ConsoleKey.X => "X",
                    ConsoleKey.Y => "Y",
                    ConsoleKey.Z => "Z",
                    ConsoleKey.Spacebar => string.Empty,
                    _ => null,
                };
                if (rotationAxis == null) break;

                Console.WriteLine(string.IsNullOrWhiteSpace(rotationAxis)
                    ? "Stopping rotation."
                    : $"Change rotation axis to: {rotationAxis}.");
            }

            // Calculate the elapsed time in seconds
            double elapsedTime = DateTime.UtcNow.TimeOfDay.TotalSeconds;

            // Calculate the rotation angle in radians (full rotation is 2 * PI)
            double angle = (elapsedTime % rotationPeriod) / rotationPeriod * 2 * Math.PI;

            float sin = (float)Math.Sin(angle / 2.0f);
            float cos = (float)Math.Cos(angle / 2.0f);

            for (byte i = 0; i < opts.Sensors; i++)
            {
                // Compute the quaternion for rotation around the axis
                var success = rotationAxis switch
                {
                    "X" => tracker.SendRotationQuatSensor(i, sin, 0.0f, 0.0f, cos),
                    "Y" => tracker.SendRotationQuatSensor(i, 0.0f, sin, 0.0f, cos),
                    "Z" => tracker.SendRotationQuatSensor(i, 0.0f, 0.0f, sin, cos),
                    _ => tracker.SendRotationQuatSensor(i, 0.0f, 0.0f, 0.0f, 1.0f)
                };
            };

            // Optional delay to avoid sending too frequently
            Thread.Sleep(10); // Adjust sleep time as necessary for your applicatio
        }
    }
}


