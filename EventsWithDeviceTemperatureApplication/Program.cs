namespace EventsWithDeviceTemperatureApplication;

//Device class and interface

interface IDevice
{
    void StartDevice();
}

class Device : IDevice
{
    public void StartDevice()
    {
        Console.WriteLine("Device is running");
    }
}

//CoolingMechanism class and interface

//ThermoStat class and interface

//HeatingSensor class and interface (with relevant event declarations with event list

public class Program{
    public static void Main(string[] args){
        Console.WriteLine("Press any key to run the device.");
        Console.ReadKey();

        IDevice device = new Device();
        device.StartDevice();

        Console.ReadKey();
    }
}