# Standalone Error Validation Module

This module provides a robust, decoupled error-calculation and validation engine for science experiment setups.

## 📌 Features
- **Validation Calculations**: Determines absolute and percentage errors using standard formulae.
- **Color-Coded Statuses**: Categorizes measurements into `Acceptable` (<=5% error), `Warning` (<=10% error), and `Failed` (>10% error).
- **Corrective Guidance**: Emits structured hints to help students correct setup or measurement issues.
- **DIP-Compliant Interface**: Uses C# interfaces (`IMeasurementReceiver`, `IValidator`) to communicate with other telemetry systems without tight coupling.

## 🗂️ Files Included
- `ErrorValidator.cs`: Main script containing validation logic.
- `ValidationResult.cs`: Holds output data (measured, correct, absolute error, % error, status, message).
- `ValidationStatus.cs`: Enum definition (`Acceptable`, `Warning`, `Failed`).
- `IMeasurementReceiver.cs`: Standardized interface for receiving telemetry data.
- `MeasurementBridge.cs`: Direct API entry point for external components.
- `ValidationUIManager.cs`: Renders the results on-screen with modern styling and UI animations.
- `UILayoutFixer.cs`: Enforces dark, premium styling onto the layout panels at runtime.

## 🚀 How to Use (Usage Instructions)
1. Attach `ErrorValidator` to a manager GameObject in your scene.
2. Setup your target `correctValue`, `acceptablePercent`, and `warningPercent` thresholds in the Unity inspector.
3. Wire up UI elements to `ValidationUIManager` and point it to your `ErrorValidator`.
4. Any measurement device (caliper, scale, etc.) can send measurements via:
   ```csharp
   measurementBridge.SendMeasurement(value, "Caliper");
   ```
