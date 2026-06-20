using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// MeasurementBridge — Public API entry point for teammate integration.
///
/// SRP: ONLY responsible for receiving external measurement data
///      and forwarding it to ErrorValidator.
///
/// WHY this class exists:
///   Teammates' scripts (VernierCaliper, ScrewGauge) call SendMeasurement()
///   directly. They never need to know about ErrorValidator — they talk to
///   this bridge. This follows the Dependency Inversion Principle.
///
/// NOTE: The Validate button is handled exclusively by ExperimentManager,
///       which enforces apparatus checks first. MeasurementBridge does NOT
///       wire to any button to avoid duplicate-listener conflicts.
/// </summary>
public class MeasurementBridge : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ErrorValidator errorValidator;

    // ── Public API for teammates ──────────────────────────────────

    /// <summary>
    /// Called by Vernier Caliper or Screw Gauge scripts to submit a reading.
    /// Example: measurementBridge.SendMeasurement(10.25f, "VernierCaliper");
    /// </summary>
    public void SendMeasurement(float value, string source = "Unknown")
    {
        if (errorValidator == null)
        {
            Debug.LogError("[MeasurementBridge] ErrorValidator is not assigned!");
            return;
        }

        Debug.Log($"[MeasurementBridge] Received {value} mm from '{source}'");
        errorValidator.ReceiveMeasurement(value);
    }
}