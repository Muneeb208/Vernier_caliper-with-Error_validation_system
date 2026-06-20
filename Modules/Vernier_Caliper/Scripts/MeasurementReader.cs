using UnityEngine;
using TMPro;

public class MeasurementReader : MonoBehaviour
{
    public Transform vernierSlider;
    public TMP_Text displayText;
    public float minX = -4f;
    public float maxX = 4.27f;
    public float maxMM = 150f;

    // Expose numeric value directly for integration
    public float CurrentValue { get; private set; }

    void Update()
    {
        if (vernierSlider == null) return;

        float t = Mathf.InverseLerp(minX, maxX, vernierSlider.position.x);
        float mm = t * maxMM;
        float rounded = Mathf.Round(mm * 10f) / 10f;
        
        CurrentValue = rounded;

        if (displayText != null)
        {
            displayText.text = rounded.ToString("F1") + " mm";
        }
    }
}
