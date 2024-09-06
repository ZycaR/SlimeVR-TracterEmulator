# C# SlimeVR Tracker Emulator

This is a console application writem in C# to emulate a SlimeVR Tracker.

## How to Clone and Build

1. **Clone the repository:**
   ```bash
   git clone ZycaR/SlimeVR-TracterEmulator
   ```
2. **Open the project in Visual Studio:**
   - Open the `.sln` file in Visual Studio.

3. **Build the application:**
   - Simply build the project in Visual Studio. No special steps are required.

## Usage

1. For a single unmoving tracker:

    Simply execute the generated executable without any arguments.

2. To emulate more tracker extensions, use the following arguments:

```bash
SlimeVR-TracterEmulator.exe -- --deviceid <number> --sensors <number> --rotation <X|Y|Z>
```

### In-Application Controls

- **Press X, Y, or Z** to change the rotation axis.
- **Press SPACE** to stop the rotation.
- **Press any other key** to exit the application.

--- 