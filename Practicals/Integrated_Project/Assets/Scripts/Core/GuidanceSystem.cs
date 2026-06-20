using UnityEngine;

// Single Responsibility: ONLY generates guidance hints
// Takes results from all validators and produces helpful messages
public class GuidanceSystem : MonoBehaviour
{
    // Generates specific hint based on apparatus result
    public string GetApparatusGuidance(ApparatusResult result)
    {
        if (result.AllChecksPassed)
            return "Setup complete! You may now take your measurement.";

        if (!result.IsVernierPresent)
            return "Ensure the Vernier Caliper is ready before starting.";

        if (!result.IsObjectPlaced)
            return "Place the object firmly between the jaws of the caliper.";

        if (!result.IsInstrumentZeroed)
            return "Zero your instrument before measuring to avoid zero error.";

        if (!result.IsCorrectScale)
            return "Select the correct scale: mm for the caliper.";

        return "Check all setup conditions before measuring.";
    }

    // Generates hint based on measurement validation result
    public string GetMeasurementGuidance(ValidationResult result)
    {
        switch (result.Status)
        {
            case ValidationStatus.Acceptable:
                return "Excellent! Your technique is correct.";

            case ValidationStatus.Warning:
                return "Slight error detected. Check jaw pressure and " +
                       "ensure you're reading the correct division.";

            case ValidationStatus.Failed:
                return "Large error. Verify: correct object in jaws, " +
                       "instrument zeroed, and reading main + vernier scale together.";

            default:
                return "Take a careful measurement.";
        }
    }
}
