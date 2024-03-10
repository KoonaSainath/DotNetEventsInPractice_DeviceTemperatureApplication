using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EventsWithDeviceTemperatureApplication;

class TemperatureData : EventArgs
{
    public double CurrentTemperature { get; set; }
    public DateTime CurrentDateTime { get; set; }
}
static class Factory
{
    private const double WARNING_TEMPERATURE_LIMIT = 27;
    private const double EMERGENCY_TEMPERATURE_LIMIT = 75;
    public static void Start()
    {
        ICoolingMechanism coolingMechanism = new CoolingMechanism();
        IThermoStat thermoStat = new ThermoStat(WARNING_TEMPERATURE_LIMIT,EMERGENCY_TEMPERATURE_LIMIT, coolingMechanism);
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
        Console.WriteLine("Device is running");
        this.thermoStat.TurnOnHeatingSensor();
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
        Console.WriteLine("Temperature exceeded warning limit!");
        Console.WriteLine("Cooling mechanism turned 'ON'");
    }
    public void Off()
    {
        Console.WriteLine("Temperature got under control. Now temperature is under the warning limit! Great!!");
        Console.WriteLine("Cooling mechanism turned 'OFF'");
    }
}

//ThermoStat class and interface
interface IThermoStat
{
    ICoolingMechanism CoolingMechanism { get; }
    void TurnOnHeatingSensor();
    void SubscribeToHeatingSensorEvents(IHeatingSensor heatingSensor);
}

class ThermoStat : IThermoStat
{
    private double warningTemperatureLimit;
    private double emergencyTemperatureLimit;
    private ICoolingMechanism coolingMechanism;
    #region CoolingMechanism property getter and setter
    public ICoolingMechanism CoolingMechanism
    {
        get
        {
            return coolingMechanism;
        }
        set
        {
            this.coolingMechanism = value;
        }
    }
    #endregion
    public ThermoStat(double warningTemperatureLimit, double emergencyTemperatureLimit, ICoolingMechanism coolingMechanism)
    {
        this.warningTemperatureLimit = warningTemperatureLimit;
        this.emergencyTemperatureLimit = emergencyTemperatureLimit;
        this.coolingMechanism = coolingMechanism;
    }
    public void TurnOnHeatingSensor()
    {
        IHeatingSensor heatingSensor = new HeatingSensor(this.warningTemperatureLimit, this.emergencyTemperatureLimit);
        SubscribeToHeatingSensorEvents(heatingSensor);
        heatingSensor.MonitorTemperature();
    }
    public void SubscribeToHeatingSensorEvents(IHeatingSensor heatingSensor)
    {
        //subscribing to relevant events
        heatingSensor.EventWarningLimitExceeded += HeatingSensor_EventWarningLimitExceeded;
        heatingSensor.EventEmergencyLimitExceeded += HeatingSensor_EventEmergencyLimitExceeded;
        heatingSensor.EventTemperatureBelowWarningLimit += HeatingSensor_EventTemperatureBelowWarningLimit;
    }
    private void HeatingSensor_EventWarningLimitExceeded(object? sender, TemperatureData e)
    {
        Console.ResetColor();
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;

        //turning on cooling mechanism
        this.coolingMechanism.On();
        Console.ResetColor();
    }

    private void HeatingSensor_EventEmergencyLimitExceeded(object? sender, TemperatureData e)
    {
        Console.ResetColor();
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.Black;

        Console.WriteLine("Emergency level exceeded!! Shutting the device down!");

        Console.ResetColor();
    }


    private void HeatingSensor_EventTemperatureBelowWarningLimit(object? sender, TemperatureData e)
    {
        Console.ResetColor();
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.ForegroundColor = ConsoleColor.Black;

        //turning off cooling mechanism
        this.coolingMechanism.Off();
        Console.ResetColor();
    }
}

//HeatingSensor class and interface (with relevant event declarations with event list
interface IHeatingSensor
{
    double WarningTemperatureLimit { get; }
    double EmergencyTemperatureLimit { get; }
    void MonitorTemperature();
    event EventHandler<TemperatureData> EventWarningLimitExceeded;
    event EventHandler<TemperatureData> EventEmergencyLimitExceeded;
    event EventHandler<TemperatureData> EventTemperatureBelowWarningLimit;
}

class HeatingSensor : IHeatingSensor
{
    private double warningTemperatureLimit;
    private double emergencyTemperatureLimit;
    private double[] temperatureValues;
    #region getters and setters
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
    private bool ExceededWarningLimit = false;
    public HeatingSensor(double warningTemperatureLimit, double emergencyTemperatureLimit)
    {
        this.warningTemperatureLimit = warningTemperatureLimit;
        this.emergencyTemperatureLimit = emergencyTemperatureLimit;
        this.temperatureValues = [2,3,5,6,8.5,9,10.8,12.6,17,18.9,25.1,26.9,27.2,28,35,31.2,25,28,23,27.5,35,42,57,83.8];
    }
    public void FireEventWarningLimitExceeded(TemperatureData temperatureData)
    {
        EventHandler<TemperatureData> handler = temperatureEventsList[KeyForEventWarningLimitExceeded] as EventHandler<TemperatureData>;
        if(handler is not null)
        {
            handler(this, temperatureData);
        }
    }
    public void FireEventEmergencyLimitExceeded(TemperatureData temperatureData)
    {
        EventHandler<TemperatureData> handler = temperatureEventsList[KeyForEventEmergencyLimitExceeded] as EventHandler<TemperatureData>;
        if(handler is not null)
        {
            handler(this, temperatureData);
        }
    }
    public void FireEventTemperatureBelowWarningLimit(TemperatureData temperatureData)
    {
        EventHandler<TemperatureData> handler = temperatureEventsList[KeyForTemperatureBelowWarningLimit] as EventHandler<TemperatureData>;
        if(handler is not null)
        {
            handler(this, temperatureData);
        }
    }
    public void MonitorTemperature()
    {
        foreach(double temperature in temperatureValues)
        {
            TemperatureData temperatureData = new TemperatureData()
            {
                CurrentTemperature = temperature,
                CurrentDateTime = DateTime.Now
            };
            Console.WriteLine($"Current temperature is: {temperatureData.CurrentTemperature} at {temperatureData.CurrentDateTime}");
            if (temperature >= this.emergencyTemperatureLimit)
            {
                FireEventEmergencyLimitExceeded(temperatureData);
            }
            else if(temperature >= this.warningTemperatureLimit)
            {
                ExceededWarningLimit = true;
                FireEventWarningLimitExceeded(temperatureData);
            }
            //Checking if temperature is below warning limit and if this is after warning limit is reached, to fire event to end cooling mechanism.
            else if(temperature < this.warningTemperatureLimit && ExceededWarningLimit)
            {
                ExceededWarningLimit = false;
                FireEventTemperatureBelowWarningLimit(temperatureData);
            }
            Thread.Sleep(1000);
        }
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