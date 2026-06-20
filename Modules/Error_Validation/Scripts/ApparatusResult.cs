using UnityEngine;

// Data class — holds result of apparatus check (SRP)
public class ApparatusResult
{
    public bool IsObjectPlaced     { get; private set; }
    public bool IsInstrumentZeroed { get; private set; }
    public bool IsCorrectScale     { get; private set; }
    public bool IsVernierPresent   { get; private set; }
    public bool IsScrewGaugePresent{ get; private set; }

    // Overall pass only if ALL checks pass
    public bool AllChecksPassed =>
        IsObjectPlaced     &&
        IsInstrumentZeroed &&
        IsCorrectScale     &&
        IsVernierPresent   &&
        IsScrewGaugePresent;

    public ApparatusResult(bool objectPlaced, bool zeroed,
                           bool correctScale, bool vernier,
                           bool screwGauge)
    {
        IsObjectPlaced      = objectPlaced;
        IsInstrumentZeroed  = zeroed;
        IsCorrectScale      = correctScale;
        IsVernierPresent    = vernier;
        IsScrewGaugePresent = screwGauge;
    }
}