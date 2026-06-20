using UnityEngine;

// Interface = a contract. Any class that implements this MUST have ReceiveMeasurement.
// This is the Dependency Inversion Principle — teammates talk to this interface,
// not directly to your ErrorValidator class.
public interface IMeasurementReceiver
{
    void ReceiveMeasurement(float measuredValue);
}