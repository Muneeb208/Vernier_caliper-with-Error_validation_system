using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Coordinator — orchestrates ALL validators (SRP: manages flow only)
// This is the main entry point for the full experiment validation
public class ExperimentManager : MonoBehaviour
{
    [Header("Validators")]
    [SerializeField] private ApparatusValidator apparatusValidator;
    [SerializeField] private ErrorValidator      errorValidator;
    [SerializeField] private GuidanceSystem      guidanceSystem;

    [Header("Checkboxes — Apparatus Setup")]
    [SerializeField] private Toggle toggle_ObjectPlaced;
    [SerializeField] private Toggle toggle_Zeroed;
    [SerializeField] private Toggle toggle_CorrectScale;
    [SerializeField] private Toggle toggle_VernierPresent;

    [Header("Measurement Input")]
    [SerializeField] private TMP_InputField inputField_Measurement;
    [SerializeField] private Button         button_Validate;

    [Header("UI Output")]
    [SerializeField] private TextMeshProUGUI text_ChecklistStatus;
    [SerializeField] private TextMeshProUGUI text_GuidanceMessage;
    [SerializeField] private TextMeshProUGUI text_ValidationResult;

    // Event — UI Manager subscribes to this
    public event System.Action<ValidationResult> OnMeasurementValidated;
    public event System.Action<string>           OnGuidanceUpdated;
    public event System.Action<ApparatusResult>  OnApparatusChecked;

    private void Start()
    {
        button_Validate.onClick.AddListener(OnValidateClicked);

        // Live checklist update when any toggle changes
        toggle_ObjectPlaced.onValueChanged.AddListener(_      => UpdateChecklist());
        toggle_Zeroed.onValueChanged.AddListener(_            => UpdateChecklist());
        toggle_CorrectScale.onValueChanged.AddListener(_      => UpdateChecklist());
        toggle_VernierPresent.onValueChanged.AddListener(_    => UpdateChecklist());

        // Sync default Inspector toggle states into the validator.
        // Without this, toggles that start ON visually are still false in ApparatusValidator.
        UpdateChecklist();
    }

    // Called live when any checkbox changes
    private void UpdateChecklist()
    {
        apparatusValidator.SetCheckboxValues(
            toggle_ObjectPlaced.isOn,
            toggle_Zeroed.isOn,
            toggle_CorrectScale.isOn,
            toggle_VernierPresent.isOn
        );

        apparatusValidator.Validate();
        ApparatusResult result = apparatusValidator.GetLastResult();

        // Update guidance live
        string guidance = guidanceSystem.GetApparatusGuidance(result);
        OnGuidanceUpdated?.Invoke(guidance);

        if (text_GuidanceMessage != null)
            text_GuidanceMessage.text = guidance;

        if (text_ChecklistStatus != null)
            text_ChecklistStatus.text = apparatusValidator.ErrorMessage;

        OnApparatusChecked?.Invoke(result);
    }

    // Called when Validate button clicked
    private void OnValidateClicked()
    {
        // Always read the latest UI state before validating.
        UpdateChecklist();

        // Step 1: Check apparatus first
        apparatusValidator.Validate();

        if (!apparatusValidator.IsValid)
        {
            if (text_ValidationResult != null)
                text_ValidationResult.text =
                    "Fix setup issues before validating measurement!";

            if (text_GuidanceMessage != null)
                text_GuidanceMessage.text =
                    guidanceSystem.GetApparatusGuidance(
                        apparatusValidator.GetLastResult());
            return;
        }

        // Step 2: Validate measurement
        if (!float.TryParse(inputField_Measurement.text, out float value))
        {
            if (text_ValidationResult != null)
                text_ValidationResult.text = "Enter a valid number!";
            return;
        }

        ValidationResult result = errorValidator.Validate(value);
        OnMeasurementValidated?.Invoke(result);

        // Notify the result UI. ValidationUIManager listens to ErrorValidator's event.
        errorValidator.ReceiveMeasurement(value);

        // Step 3: Get guidance for measurement result
        string measureGuidance = guidanceSystem.GetMeasurementGuidance(result);
        if (text_GuidanceMessage != null)
            text_GuidanceMessage.text = measureGuidance;
    }
}
