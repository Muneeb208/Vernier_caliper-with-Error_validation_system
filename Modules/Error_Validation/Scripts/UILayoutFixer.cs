using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UILayoutFixer — Auto-positions all UI panels on Start so Game View always shows
/// the correct layout regardless of how panels are placed in the Editor.
///
/// SRP: This class ONLY handles UI layout positioning.
/// Attach to: Canvas GameObject.
/// </summary>
public class UILayoutFixer : MonoBehaviour
{
    [Header("Panels to Position")]
    [SerializeField] private RectTransform panel_Title;
    [SerializeField] private RectTransform panel_Checklist;
    [SerializeField] private RectTransform panel_Input;
    [SerializeField] private RectTransform panel_Result;
    [SerializeField] private RectTransform panel_Guidance;

    // ── Premium Color Palette ────────────────────────────────────
    private static readonly Color COLOR_HEADER      = new Color(0.05f, 0.75f, 0.55f, 1f);       // Emerald header
    private static readonly Color COLOR_CHECKLIST   = new Color(0.09f, 0.10f, 0.15f, 1f);        // Deep charcoal
    private static readonly Color COLOR_INPUT       = new Color(0.07f, 0.08f, 0.12f, 1f);        // Darker charcoal
    private static readonly Color COLOR_GUIDANCE    = new Color(0.08f, 0.10f, 0.16f, 1f);        // Slate dark
    private static readonly Color COLOR_RESULT      = new Color(0.07f, 0.08f, 0.13f, 1f);        // Near-black card

    private void Awake()
    {
        FixAllPanels();
        StyleAllText();
        StyleInputFields();
        StyleToggles();
    }

    private void FixAllPanels()
    {
        // ── PANEL_TITLE ──────────────────────────────────────────
        if (panel_Title != null)
        {
            SetAnchors(panel_Title, new Vector2(0, 1), new Vector2(1, 1));
            panel_Title.anchoredPosition = new Vector2(0, 0);
            panel_Title.sizeDelta        = new Vector2(0, 75);
            SetBackground(panel_Title, COLOR_HEADER);
            AddOutline(panel_Title, new Color(0.03f, 0.55f, 0.40f, 0.6f), new Vector2(0, -3f));
        }

        // ── PANEL_CHECKLIST ──────────────────────────────────────
        if (panel_Checklist != null)
        {
            SetAnchors(panel_Checklist, new Vector2(0, 1), new Vector2(1, 1));
            panel_Checklist.anchoredPosition = new Vector2(0, -83);
            panel_Checklist.sizeDelta        = new Vector2(0, 200);
            SetBackground(panel_Checklist, COLOR_CHECKLIST);
            AddOutline(panel_Checklist, new Color(0.05f, 0.75f, 0.55f, 0.12f), new Vector2(1, 1));
        }

        // ── PANEL_INPUT ──────────────────────────────────────────
        if (panel_Input != null)
        {
            SetAnchors(panel_Input, new Vector2(0, 1), new Vector2(1, 1));
            panel_Input.anchoredPosition = new Vector2(0, -291);
            panel_Input.sizeDelta        = new Vector2(0, 100);
            SetBackground(panel_Input, COLOR_INPUT);
            AddOutline(panel_Input, new Color(0.05f, 0.75f, 0.55f, 0.08f), new Vector2(1, 1));
        }

        // ── PANEL_GUIDANCE ───────────────────────────────────────
        if (panel_Guidance != null)
        {
            SetAnchors(panel_Guidance, new Vector2(0, 1), new Vector2(1, 1));
            panel_Guidance.anchoredPosition = new Vector2(0, -399);
            panel_Guidance.sizeDelta        = new Vector2(0, 90);
            SetBackground(panel_Guidance, COLOR_GUIDANCE);
            AddOutline(panel_Guidance, new Color(0.20f, 0.55f, 0.85f, 0.15f), new Vector2(1, 1));
        }

        // ── PANEL_RESULT ─────────────────────────────────────────
        if (panel_Result != null)
        {
            SetAnchors(panel_Result, new Vector2(0, 1), new Vector2(1, 1));
            panel_Result.anchoredPosition = new Vector2(0, -497);
            panel_Result.sizeDelta        = new Vector2(0, 220);
            SetBackground(panel_Result, COLOR_RESULT);
            AddOutline(panel_Result, new Color(0.05f, 0.75f, 0.55f, 0.10f), new Vector2(1, 1));
        }

        Debug.Log("[UILayoutFixer] All panels repositioned successfully.");
    }

    /// <summary>Styles all TextMeshProUGUI text in children for a consistent premium look.</summary>
    private void StyleAllText()
    {
        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI tmp in allTexts)
        {
            // Don't override input field placeholder/text area children
            if (tmp.transform.parent != null &&
                tmp.transform.parent.name.Contains("Text Area"))
                continue;

            // Title panel — white bold, larger
            if (panel_Title != null && tmp.transform.IsChildOf(panel_Title))
            {
                tmp.color = Color.white;
                tmp.fontStyle = FontStyles.Bold;
                tmp.fontSize = Mathf.Max(tmp.fontSize, 24f);
                continue;
            }

            // Checklist status / apparatus text — bright off-white
            if (tmp.gameObject.name.Contains("Checklist") ||
                tmp.gameObject.name.Contains("Status"))
            {
                tmp.color = new Color(0.90f, 0.92f, 0.96f, 1f);
                tmp.fontSize = Mathf.Max(tmp.fontSize, 14f);
                continue;
            }

            // Guidance text — warm sky blue, italic
            if (panel_Guidance != null && tmp.transform.IsChildOf(panel_Guidance))
            {
                tmp.color = new Color(0.45f, 0.78f, 1f, 1f);
                tmp.fontStyle = FontStyles.Italic;
                tmp.fontSize = Mathf.Max(tmp.fontSize, 14f);
                continue;
            }

            // Result panel text — clean off-white
            if (panel_Result != null && tmp.transform.IsChildOf(panel_Result))
            {
                tmp.color = new Color(0.88f, 0.90f, 0.93f, 1f);
                tmp.fontSize = Mathf.Max(tmp.fontSize, 15f);
                continue;
            }

            // Default label color — soft silver
            tmp.color = new Color(0.80f, 0.84f, 0.90f, 1f);
        }

        // Style the Validate button
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            Image btnImg = btn.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = new Color(0.05f, 0.75f, 0.55f, 1f);
            }

            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.color = Color.white;
                btnText.fontStyle = FontStyles.Bold;
                btnText.fontSize = Mathf.Max(btnText.fontSize, 16f);
            }

            // Add outline glow to button
            Outline btnOutline = btn.GetComponent<Outline>();
            if (btnOutline == null) btnOutline = btn.gameObject.AddComponent<Outline>();
            btnOutline.effectColor = new Color(0.03f, 0.55f, 0.40f, 0.5f);
            btnOutline.effectDistance = new Vector2(0, -2f);
        }
    }

    /// <summary>Styles TMP_InputField components to match the dark theme.</summary>
    private void StyleInputFields()
    {
        TMP_InputField[] inputs = GetComponentsInChildren<TMP_InputField>(true);
        foreach (TMP_InputField input in inputs)
        {
            // Dark input background
            Image inputBg = input.GetComponent<Image>();
            if (inputBg != null)
                inputBg.color = new Color(0.12f, 0.14f, 0.20f, 1f);

            // Style the text inside input fields
            if (input.textComponent != null)
            {
                input.textComponent.color = new Color(0.90f, 0.92f, 0.96f, 1f);
                input.textComponent.fontSize = Mathf.Max(input.textComponent.fontSize, 16f);
            }

            // Style placeholder
            if (input.placeholder != null)
            {
                TextMeshProUGUI ph = input.placeholder as TextMeshProUGUI;
                if (ph != null)
                    ph.color = new Color(0.40f, 0.44f, 0.52f, 1f);
            }

            // Add subtle border
            Outline inputOutline = input.GetComponent<Outline>();
            if (inputOutline == null)
                inputOutline = input.gameObject.AddComponent<Outline>();
            inputOutline.effectColor = new Color(0.05f, 0.75f, 0.55f, 0.20f);
            inputOutline.effectDistance = new Vector2(1, 1);
        }
    }

    /// <summary>Styles Toggle components with dark checkmark backgrounds.</summary>
    private void StyleToggles()
    {
        Toggle[] toggles = GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            // Style the toggle background (the checkbox square)
            Transform bgTransform = toggle.transform.Find("Background");
            if (bgTransform != null)
            {
                Image bgImg = bgTransform.GetComponent<Image>();
                if (bgImg != null)
                    bgImg.color = new Color(0.15f, 0.17f, 0.24f, 1f);

                // Style the checkmark
                Transform checkmark = bgTransform.Find("Checkmark");
                if (checkmark != null)
                {
                    Image checkImg = checkmark.GetComponent<Image>();
                    if (checkImg != null)
                        checkImg.color = new Color(0.05f, 0.82f, 0.55f, 1f); // Emerald check
                }
            }

            // Style toggle label text
            TextMeshProUGUI label = toggle.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.color = new Color(0.82f, 0.85f, 0.90f, 1f);
                label.fontSize = Mathf.Max(label.fontSize, 14f);
            }
        }
    }

    // ── Helpers ──────────────────────────────────────────────────

    private void SetAnchors(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.pivot     = new Vector2(0.5f, 1f);
    }

    private void SetBackground(RectTransform rt, Color color)
    {
        Image img = rt.GetComponent<Image>();
        if (img == null)
            img = rt.gameObject.AddComponent<Image>();
        img.color = color;
    }

    private void AddOutline(RectTransform rt, Color color, Vector2 distance)
    {
        Outline outline = rt.GetComponent<Outline>();
        if (outline == null)
            outline = rt.gameObject.AddComponent<Outline>();
        outline.effectColor    = color;
        outline.effectDistance = distance;
    }
}
