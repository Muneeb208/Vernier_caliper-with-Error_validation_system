using UnityEngine;

// Single Responsibility: only holds the result data (Data Transfer Object pattern)
public class ValidationResult
{
    public float MeasuredValue    { get; private set; }
    public float CorrectValue     { get; private set; }
    public float AbsoluteError    { get; private set; }
    public float PercentageError  { get; private set; }
    public ValidationStatus Status { get; private set; }
    public string Message         { get; private set; }

    // Constructor enforces that a result is always fully formed
    public ValidationResult(float measured, float correct, float absoluteError,
                            float percentageError, ValidationStatus status, string message)
    {
        MeasuredValue   = measured;
        CorrectValue    = correct;
        AbsoluteError   = absoluteError;
        PercentageError = percentageError;
        Status          = status;
        Message         = message;
    }
}