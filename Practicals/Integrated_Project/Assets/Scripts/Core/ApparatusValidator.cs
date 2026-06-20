using UnityEngine;

// Checks if apparatus is set up correctly before measurement
// Implements IValidator — follows Dependency Inversion Principle
public class ApparatusValidator : MonoBehaviour, IValidator
{
    // These will be set by UI checkboxes via ExperimentManager
    private bool _objectPlaced;
    private bool _instrumentZeroed;
    private bool _correctScale;
    private bool _vernierPresent;

    // IValidator implementation
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; }

    // Called by ExperimentManager with checkbox values
    public void SetCheckboxValues(bool objectPlaced, bool zeroed,
                                  bool correctScale, bool vernier)
    {
        _objectPlaced       = objectPlaced;
        _instrumentZeroed   = zeroed;
        _correctScale       = correctScale;
        _vernierPresent     = vernier;
    }

    public void Validate()
    {
        ApparatusResult result = new ApparatusResult(
            _objectPlaced, _instrumentZeroed, _correctScale,
            _vernierPresent
        );

        IsValid      = result.AllChecksPassed;
        ErrorMessage = BuildErrorMessage(result);
    }

    // Builds specific error message based on what failed
    private string BuildErrorMessage(ApparatusResult result)
    {
        if (result.AllChecksPassed)
            return "All apparatus checks passed!";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Setup Issues Detected:");

        if (!result.IsVernierPresent)
            sb.AppendLine("  - Vernier Caliper not present");
        if (!result.IsObjectPlaced)
            sb.AppendLine("  - Object not placed in jaws");
        if (!result.IsInstrumentZeroed)
            sb.AppendLine("  - Instrument not zeroed");
        if (!result.IsCorrectScale)
            sb.AppendLine("  - Wrong scale selected");

        return sb.ToString();
    }

    public ApparatusResult GetLastResult()
    {
        return new ApparatusResult(
            _objectPlaced, _instrumentZeroed, _correctScale,
            _vernierPresent
        );
    }
}
