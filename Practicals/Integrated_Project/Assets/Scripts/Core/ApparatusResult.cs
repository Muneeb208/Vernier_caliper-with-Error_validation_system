using UnityEngine;

// Data class — holds result of apparatus check (SRP)
public class ApparatusResult
{
    public bool IsObjectPlaced     { get; private set; }
    public bool IsInstrumentZeroed { get; private set; }
    public bool IsCorrectScale     { get; private set; }
    public bool IsVernierPresent   { get; private set; }

    // Overall pass only if ALL checks pass
    public bool AllChecksPassed =>
        IsObjectPlaced     &&
        IsInstrumentZeroed &&
        IsCorrectScale     &&
        IsVernierPresent;

    public ApparatusResult(bool objectPlaced, bool zeroed,
                           bool correctScale, bool vernier)
    {
        IsObjectPlaced      = objectPlaced;
        IsInstrumentZeroed  = zeroed;
        IsCorrectScale      = correctScale;
        IsVernierPresent    = vernier;
    }
}