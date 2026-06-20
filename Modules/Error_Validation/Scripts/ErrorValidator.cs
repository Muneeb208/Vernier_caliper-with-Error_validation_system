using UnityEngine;

// Single Responsibility: only validates measurements
// Implements IMeasurementReceiver so teammates can send data to it
public class ErrorValidator : MonoBehaviour, IMeasurementReceiver
{
    [Header("Validation Settings")]
    [SerializeField] private float correctValue = 10.0f;        // The true answer
    [SerializeField] private float acceptablePercent = 5.0f;    // e.g. within 5% = Acceptable
    [SerializeField] private float warningPercent = 10.0f;      // e.g. within 10% = Warning
                                                                 // beyond 10% = Failed

    // Event: other classes can subscribe to get results without tight coupling
    public event System.Action<ValidationResult> OnValidationComplete;

    // Called by teammates to send a measurement
    public void ReceiveMeasurement(float measuredValue)
    {
        ValidationResult result = Validate(measuredValue);
        OnValidationComplete?.Invoke(result);  // notify anyone listening
    }

    // Core logic — pure calculation, easy to unit test
    public ValidationResult Validate(float measuredValue)
    {
        float absoluteError    = Mathf.Abs(measuredValue - correctValue);
        float percentageError  = (correctValue != 0)
                                 ? (absoluteError / Mathf.Abs(correctValue)) * 100f
                                 : 0f;

        ValidationStatus status;
        string message;

        if (percentageError <= acceptablePercent)
        {
            status  = ValidationStatus.Acceptable;
            message = "Great measurement! Error is within acceptable range.";
        }
        else if (percentageError <= warningPercent)
        {
            status  = ValidationStatus.Warning;
            message = "Measurement is slightly off. Check your technique.";
        }
        else
        {
            status  = ValidationStatus.Failed;
            message = "Measurement too far from correct value. Try again.";
        }

        return new ValidationResult(measuredValue, correctValue,
                                    absoluteError, percentageError,
                                    status, message);
    }
}