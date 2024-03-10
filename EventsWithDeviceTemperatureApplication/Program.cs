using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EventsWithDeviceTemperatureApplication;

//Device class and interface

static class Factory
{
    private const double WARNING_TEMPERATURE_LIMIT = 27;
    private const double EMERGENCY_TEMPERATURE_LIMIT = 75;
    public static void Start()
    {
        IThermoStat thermoStat = new ThermoStat(WARNING_TEMPERATURE_LIMIT,EMERGENCY_TEMPERATURE_LIMIT);
        IDevice device = new Device(thermoStat);
        device.StartDevice();
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
        this.thermoStat.TurnOnHeatingSensor();
        Console.WriteLine("Device is running");
    }
}

//CoolingMechanism class and interface
interface ICoolingMechanism
{
    void On();
    void Off();
}

class CoolingMechanism : ICoolingMechanism
{
    public void On()
    {
        Console.WriteLine("Cooling mechanism turned 'ON'");
    }
    public void Off()
    {
        Console.WriteLine("Cooling mechanism turned 'OFF'");
    }
}

//ThermoStat class and interface
interface IThermoStat
{
    void TurnOnHeatingSensor();
}

class ThermoStat : IThermoStat
{
    private double warningTemperatureLimit;
    private double emergencyTemperatureLimit;
    public ThermoStat(double warningTemperatureLimit, double emergencyTemperatureLimit)
    {
        this.warningTemperatureLimit = warningTemperatureLimit;
        this.emergencyTemperatureLimit = emergencyTemperatureLimit;
    }
    public void TurnOnHeatingSensor()
    {
        ICoolingMechanism coolingMechanism = new CoolingMechanism();
        IHeatingSensor heatingSensor = new HeatingSensor(coolingMechanism, this.warningTemperatureLimit, this.emergencyTemperatureLimit);
        heatingSensor.MonitorTemperature();
    }
}

//HeatingSensor class and interface (with relevant event declarations with event list
interface IHeatingSensor
{
    double WarningTemperatureLimit { get; }
    double EmergencyTemperatureLimit { get; }
    ICoolingMechanism CoolingMechanism { get; }
    void MonitorTemperature();
    event EventHandler<TemperatureData> EventWarningLimitExceeded;
    event EventHandler<TemperatureData> EventEmergencyLimitExceeded;
    event EventHandler<TemperatureData> EventTemperatureBelowWarningLimit;
}

class TemperatureData : EventArgs
{
    public double CurrentTemperature { get; set; }
    public DateTime CurrentDateTime { get; set; } 
}

class HeatingSensor : IHeatingSensor
{
    private ICoolingMechanism coolingMechanism;
    private double warningTemperatureLimit;
    private double emergencyTemperatureLimit;
    private double[] temperatureData;
    #region getters and setters
    #region CoolingMechanism property with custom getter and setter
    public ICoolingMechanism CoolingMechanism
    {
        get
        {
            return this.coolingMechanism;
        }
        set
        {
            this.coolingMechanism = value;
        }
    }
    #endregion
    #region WarningTemperatureLimit property with custom getter and setter
    public double WarningTemperatureLimit
    {
        get
        {
            return this.warningTemperatureLimit;
        }
        set
        {
            this.warningTemperatureLimit = value;
        }
    }
    #endregion
    #region EmergencyTemperatureLimit property with custom getter and setter
    public double EmergencyTemperatureLimit
    {
        get
        {
            return this.emergencyTemperatureLimit;
        }
        set
        {
            this.emergencyTemperatureLimit = value;
        }
    }
    #endregion
    #endregion

    //EventHandlerList holds multiple events in list datastructure with key objects and event values.
    private EventHandlerList temperatureEventsList = new EventHandlerList();

    private object KeyForEventWarningLimitExceeded = new object();
    private object KeyForEventEmergencyLimitExceeded = new object();
    private object KeyForTemperatureBelowWarningLimit = new object();
    #region Declared events
    public event EventHandler<TemperatureData> EventWarningLimitExceeded
    {
        //Executes when calling code subscribes to this event
        add
        {
            //First parameter is "key" and second parameter is "value"
            //Key is the object we specifically declared for this event
            //"value" is passed directly when calling code subscribes to this event. When calling code subscribes to this event, code knows what event it is subscribing to, and passes that event to "value".
            temperatureEventsList.AddHandler(KeyForEventWarningLimitExceeded, value);
        }
        //Executes when calling code unsubscribes to this event
        remove
        {
            //Removing the event from the EventHandlerList.
            //Gets executed only when calling code unsubscribes from this event.
            temperatureEventsList.RemoveHandler(KeyForEventWarningLimitExceeded, value);
        }
    }
    public event EventHandler<TemperatureData> EventEmergencyLimitExceeded
    {
        //Executes when calling code subscribes to this event
        add
        {
            //First parameter is "Key" and second parameter is "value"
            //Key is the object we specifically declared for this event
            //"value" is passed directly when calling code subscribes to this event. When calling code subscribes to this event, code knows what event it is subscribing to, and passes that event to "value".
            temperatureEventsList.AddHandler(KeyForEventEmergencyLimitExceeded, value);
        }
        //Executes when calling code unsubscribes to this event
        remove
        {
            //Removing the event from the EventHandlerList.
            //Gets executed only when calling code unsubscribes from this event.
            temperatureEventsList.RemoveHandler(KeyForEventEmergencyLimitExceeded, value);
        }
    }
    public event EventHandler<TemperatureData> EventTemperatureBelowWarningLimit
    {
        //Executes when calling code subscribes to this event
        add
        {
            //First parameter is "Key" and second parameter is "value".
            //Key is the object we specifically declared for this event.
            //"value" is passed directly when calling code subscribes to this event. When calling code subscribes to this event, code knows what event it is subscribing to, and passes that event to "value"
            temperatureEventsList.AddHandler(KeyForTemperatureBelowWarningLimit, value);
        }
        //Executes when calling code unsubscribes to this event
        remove
        {
            //Removing the event from the EventHandlerList.
            //Gets executed only when calling code unsubscribes from this event
            temperatureEventsList.RemoveHandler(KeyForTemperatureBelowWarningLimit, value);
        }
    }
    #endregion
    public HeatingSensor(ICoolingMechanism coolingMechanism, double warningTemperatureLimit, double emergencyTemperatureLimit)
    {
        this.coolingMechanism = coolingMechanism;
        this.warningTemperatureLimit = warningTemperatureLimit;
        this.emergencyTemperatureLimit = emergencyTemperatureLimit;
        this.temperatureData = [2,3,5,6,8.5,9,10.8,12.6,17,18.9,25.1,26.9,27.2,28,35,31.2,25,28,23,27.5,35,42,57,83.8];
    }
    public void MonitorTemperature()
    {

    }
}

public class Program{
    public static void Main(string[] args){
        Console.WriteLine("Press any key to run the device.");
        Console.ReadKey();

        Factory.Start();

        Console.ReadKey();
    }
}