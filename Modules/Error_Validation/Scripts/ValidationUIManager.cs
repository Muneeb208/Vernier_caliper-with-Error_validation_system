using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Single Responsibility: ONLY handles displaying results on screen
// It knows nothing about how validation works — just how to show it
public class ValidationUIManager : MonoBehaviour
{
    [Header("Result Panel Texts")]
    [SerializeField] private TextMeshProUGUI text_Status;
    [SerializeField] private TextMeshProUGUI text_MeasuredValue;
    [SerializeField] private TextMeshProUGUI text_AbsoluteError;
    [SerializeField] private TextMeshProUGUI text_PercentageError;
    [SerializeField] private TextMeshProUGUI text_Message;

    [Header("Dependencies")]
    [SerializeField] private ErrorValidator errorValidator;

    // ── Premium Status Colors ────────────────────────────────────
    // Curated, modern colors instead of raw Color.green/yellow/red
    private static readonly Color COLOR_ACCEPTABLE = new Color(0.05f, 0.82f, 0.55f, 1f);   // Emerald green
    private static readonly Color COLOR_WARNING    = new Color(1f, 0.76f, 0.15f, 1f);       // Warm amber gold
    private static readonly Color COLOR_FAILED     = new Color(0.95f, 0.28f, 0.30f, 1f);    // Soft coral red

    // Result panel background tints (subtle color wash behind results)
    private static readonly Color BG_ACCEPTABLE = new Color(0.03f, 0.18f, 0.12f, 0.95f);  // Dark emerald tint
    private static readonly Color BG_WARNING    = new Color(0.18f, 0.15f, 0.04f, 0.95f);   // Dark amber tint
    private static readonly Color BG_FAILED     = new Color(0.20f, 0.06f, 0.06f, 0.95f);   // Dark red tint

    private Image resultPanelBg;

    private void Awake()
    {
        // Cache reference to the result panel background for color-coding
        Transform parent = text_Status != null ? text_Status.transform.parent : null;
        if (parent != null)
            resultPanelBg = parent.GetComponent<Image>();
    }

    private void OnEnable()
    {
        // Subscribe to the validator's event
        if (errorValidator != null)
            errorValidator.OnValidationComplete += DisplayResult;
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks
        if (errorValidator != null)
            errorValidator.OnValidationComplete -= DisplayResult;
    }

    public void DisplayResult(ValidationResult result)
    {
        // ── Format result data with clean typography ──
        text_MeasuredValue.text   = $"Measured: {result.MeasuredValue:F3} mm";
        text_AbsoluteError.text   = $"Absolute Error: {result.AbsoluteError:F3} mm";
        text_PercentageError.text = $"Error %: {result.PercentageError:F2}%";
        text_Message.text         = result.Message;

        // Style data labels consistently
        Color dataColor = new Color(0.82f, 0.85f, 0.90f, 1f);
        text_MeasuredValue.color   = dataColor;
        text_AbsoluteError.color   = dataColor;
        text_PercentageError.color = dataColor;
        text_Message.color         = new Color(0.72f, 0.75f, 0.80f, 1f);
        text_Message.fontStyle     = FontStyles.Italic;

        // ── Color-coded status with panel background tint ──
        Color statusColor;
        Color bgColor;
        string statusLabel;
        string statusIcon;

        switch (result.Status)
        {
            case ValidationStatus.Acceptable:
                statusColor = COLOR_ACCEPTABLE;
                bgColor     = BG_ACCEPTABLE;
                statusLabel = "ACCEPTABLE";
                statusIcon  = ">> ";
                break;
            case ValidationStatus.Warning:
                statusColor = COLOR_WARNING;
                bgColor     = BG_WARNING;
                statusLabel = "WARNING";
                statusIcon  = ">> ";
                break;
            case ValidationStatus.Failed:
            default:
                statusColor = COLOR_FAILED;
                bgColor     = BG_FAILED;
                statusLabel = "FAILED";
                statusIcon  = ">> ";
                break;
        }

        text_Status.text      = statusIcon + statusLabel;
        text_Status.color     = statusColor;
        text_Status.fontStyle = FontStyles.Bold;
        text_Status.fontSize  = 28f;

        // Tint the result panel background to match the status
        if (resultPanelBg != null)
            resultPanelBg.color = bgColor;

        // Animate the status text with a scale pop
        StopAllCoroutines();
        StartCoroutine(AnimateStatusPop());
    }

    private System.Collections.IEnumerator AnimateStatusPop()
    {
        if (text_Status == null) yield break;

        // Quick scale-up then settle back
        text_Status.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        float elapsed = 0f;
        float duration = 0.25f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease-out bounce effect
            float scale = Mathf.Lerp(1.2f, 1f, t * t);
            text_Status.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        text_Status.transform.localScale = Vector3.one;
    }
}
