using System.Runtime.CompilerServices;

namespace EventsWithDeviceTemperatureApplication;

//Device class and interface

static class Factory
{
    public static void Start()
    {
        
    }
}

interface IDevice
{
    void StartDevice();
}

class Device : IDevice
{
    private IThermoStat thermoStat;
    public Device(IThermoStat thermoStat)
    {
        this.thermoStat = thermoStat;
    }
    public void StartDevice()
    {
        Console.WriteLine("Device is running");
    }
}

//CoolingMechanism class and interface
interface ICoolingMechanism
{

}

//ThermoStat class and interface
interface IThermoStat
{

}

//HeatingSensor class and interface (with relevant event declarations with event list
interface IHeatingSensor
{

}

public class Program{
    public static void Main(string[] args){
        Console.WriteLine("Press any key to run the device.");
        Console.ReadKey();

        Factory.Start();

        Console.ReadKey();
    }
}