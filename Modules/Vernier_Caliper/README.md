# Standalone Vernier Caliper Simulator Module

This module provides an interactive 2D simulation of a Vernier Caliper, complete with sliding jaws, scale generation, and digital readout computation.

## 📌 Features
- **Smooth dragging jaws**: Uses raycasting to let users drag the slide jaws on the ruler track.
- **Dynamic Scale Generation**: Procedurally draws centimeter, half-centimeter, and millimeter ticks using texture generation.
- **Accurate Readings**: Computes Vernier division alignments up to a maximum span of 150mm.
- **Pulsing UI Readout**: Scales text elements on measurement change for a high-fidelity feel.

## 🗂️ Files Included
- `CaliperSlider.cs`: Manages mouse drag boundaries and raises release callbacks.
- `MeasurementReader.cs`: Computes physical reading in mm by mapping current slide position between world-coordinate limits.
- `VernierIntegration.cs`: Orchestrates UI creation, draws rulers procedurally, and dispatches data to the main application bridge.

## 🚀 How to Use (Usage Instructions)
1. Add the Vernier Caliper prefab into your UI Canvas.
2. The slider uses physics events, so ensure a `Physics2DRaycaster` is attached to your Main Camera:
   ```csharp
   Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
   ```
3. Subscribe to slider release callbacks to read measurements:
   ```csharp
   caliperSlider.OnDragReleased += () => {
       float val = measurementReader.CurrentValue;
       Debug.Log("Measurement is " + val);
   };
   ```
