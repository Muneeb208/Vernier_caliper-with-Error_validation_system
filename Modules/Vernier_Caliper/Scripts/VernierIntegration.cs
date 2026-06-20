#pragma warning disable 618
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VernierIntegration : MonoBehaviour
{
    private GameObject vernierPanel;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI readoutText;
    private Image trackImage;
    private Image sliderImage;
    
    private GameObject startLimitObj;
    private GameObject endLimitObj;

    private CaliperSlider caliperSlider;
    private MeasurementReader measurementReader;

    private TMP_InputField targetInputField;
    private MeasurementBridge measurementBridge;
    
    private float lastValue = -1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        // Avoid duplicate initialization across scene loads
        if (GameObject.Find("[VernierCaliperManager]") != null) return;
        
        GameObject manager = new GameObject("[VernierCaliperManager]");
        manager.AddComponent<VernierIntegration>();
        DontDestroyOnLoad(manager);
    }

    void Start()
    {
        InitializeIntegration();
    }

    private void InitializeIntegration()
    {
        // 1. Configure Canvas Render Mode to Screen Space - Camera for Raycasting
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 10f; // Align Canvas at z = 0 with camera at z = -10
            Debug.Log("[VernierIntegration] Configured Canvas to ScreenSpaceCamera at plane distance 10");
        }

        // 2. Add Raycaster to Main Camera for OnMouseDown to work on UI colliders
        if (Camera.main != null && Camera.main.gameObject.GetComponent<Physics2DRaycaster>() == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
            Debug.Log("[VernierIntegration] Added Physics2DRaycaster to Main Camera");
        }

        // 3. Find parent Panel_Checklist to nest the simulator
        GameObject checklistObj = GameObject.Find("Panel_Checklist");
        if (checklistObj == null)
        {
            Debug.LogError("[VernierIntegration] Could not find Panel_Checklist in the scene!");
            return;
        }

        // 4. Resolve dependencies
        GameObject inputObj = GameObject.Find("InputField_Measurement");
        if (inputObj != null)
        {
            targetInputField = inputObj.GetComponent<TMP_InputField>();
        }

        measurementBridge = FindFirstObjectByType<MeasurementBridge>();

        // 5. Create Vernier Panel (Glassmorphic dark blue theme)
        vernierPanel = new GameObject("VernierPanel", typeof(RectTransform));
        vernierPanel.transform.SetParent(checklistObj.transform, false);
        
        RectTransform panelRt = vernierPanel.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(1f, 1f);
        panelRt.anchorMax = new Vector2(1f, 1f);
        panelRt.pivot = new Vector2(1f, 1f);
        panelRt.anchoredPosition = new Vector2(-20f, -10f);
        panelRt.sizeDelta = new Vector2(360f, 180f);

        Image panelBg = vernierPanel.AddComponent<Image>();
        panelBg.color = new Color(0.06f, 0.07f, 0.11f, 0.97f);
        
        Outline outline = vernierPanel.AddComponent<Outline>();
        outline.effectColor = new Color(0.05f, 0.75f, 0.55f, 0.25f);  // Emerald glow
        outline.effectDistance = new Vector2(1.5f, 1.5f);

        // 6. Title Label
        GameObject titleObj = new GameObject("TitleText", typeof(RectTransform));
        titleObj.transform.SetParent(vernierPanel.transform, false);
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 1f);
        titleRt.anchorMax = new Vector2(0f, 1f);
        titleRt.pivot = new Vector2(0f, 1f);
        titleRt.anchoredPosition = new Vector2(15f, -15f);
        titleRt.sizeDelta = new Vector2(200f, 25f);

        titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "VERNIER CALIPER";
        titleText.fontSize = 11;
        titleText.fontStyle = FontStyles.Bold;
        titleText.characterSpacing = 3f;
        titleText.color = new Color(0.55f, 0.82f, 0.72f, 0.9f);  // Soft emerald

        // 7. Reading Digital Display
        GameObject readoutObj = new GameObject("ReadoutText", typeof(RectTransform));
        readoutObj.transform.SetParent(vernierPanel.transform, false);
        RectTransform readoutRt = readoutObj.GetComponent<RectTransform>();
        readoutRt.anchorMin = new Vector2(1f, 1f);
        readoutRt.anchorMax = new Vector2(1f, 1f);
        readoutRt.pivot = new Vector2(1f, 1f);
        readoutRt.anchoredPosition = new Vector2(-15f, -15f);
        readoutRt.sizeDelta = new Vector2(100f, 30f);

        readoutText = readoutObj.AddComponent<TextMeshProUGUI>();
        readoutText.text = "0.0 mm";
        readoutText.fontSize = 18;
        readoutText.fontStyle = FontStyles.Bold;
        readoutText.alignment = TextAlignmentOptions.Right;
        readoutText.color = new Color(0.05f, 0.82f, 0.55f, 1f);  // Emerald readout

        // 8. Fixed Scale (Main Scale Track)
        GameObject trackObj = new GameObject("RulerTrack", typeof(RectTransform));
        trackObj.transform.SetParent(vernierPanel.transform, false);
        RectTransform trackRt = trackObj.GetComponent<RectTransform>();
        trackRt.anchorMin = new Vector2(0f, 0.5f);
        trackRt.anchorMax = new Vector2(0f, 0.5f);
        trackRt.pivot = new Vector2(0f, 0.5f);
        trackRt.anchoredPosition = new Vector2(15f, -15f);
        trackRt.sizeDelta = new Vector2(330f, 40f);

        trackImage = trackObj.AddComponent<Image>();
        Texture2D mainScaleTex = GenerateMainScaleTexture();
        trackImage.sprite = Sprite.Create(mainScaleTex, new Rect(0, 0, mainScaleTex.width, mainScaleTex.height), new Vector2(0.5f, 0.5f));

        // Create empty anchors to dynamically establish min/max world bounds
        startLimitObj = new GameObject("StartLimit");
        startLimitObj.transform.SetParent(trackObj.transform, false);
        RectTransform startLimitRt = startLimitObj.AddComponent<RectTransform>();
        startLimitRt.anchorMin = new Vector2(0f, 0.5f);
        startLimitRt.anchorMax = new Vector2(0f, 0.5f);
        startLimitRt.pivot = new Vector2(0f, 0.5f);
        startLimitRt.anchoredPosition = new Vector2(0f, 0f);

        endLimitObj = new GameObject("EndLimit");
        endLimitObj.transform.SetParent(trackObj.transform, false);
        RectTransform endLimitRt = endLimitObj.AddComponent<RectTransform>();
        endLimitRt.anchorMin = new Vector2(0f, 0.5f);
        endLimitRt.anchorMax = new Vector2(0f, 0.5f);
        endLimitRt.pivot = new Vector2(0f, 0.5f);
        // The ruler track scale goes from 0 to 150mm. Each 1mm is 2 pixels = 300 pixels span.
        endLimitRt.anchoredPosition = new Vector2(270f, 0f); 

        // 9. Sliding Scale (Vernier Slide)
        GameObject sliderObj = new GameObject("SlidingJaw", typeof(RectTransform));
        sliderObj.transform.SetParent(trackObj.transform, false);
        RectTransform sliderRt = sliderObj.GetComponent<RectTransform>();
        sliderRt.anchorMin = new Vector2(0f, 0.5f);
        sliderRt.anchorMax = new Vector2(0f, 0.5f);
        sliderRt.pivot = new Vector2(0f, 0.5f);
        sliderRt.anchoredPosition = new Vector2(0f, 0f);
        sliderRt.sizeDelta = new Vector2(60f, 50f);

        sliderImage = sliderObj.AddComponent<Image>();
        Texture2D vernierScaleTex = GenerateVernierScaleTexture();
        sliderImage.sprite = Sprite.Create(vernierScaleTex, new Rect(0, 0, vernierScaleTex.width, vernierScaleTex.height), new Vector2(0.5f, 0.5f));

        // Add 2D BoxCollider to capture dragging physics events
        BoxCollider2D collider = sliderObj.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(60f, 50f);
        collider.isTrigger = true;

        // Attach dragging logic
        caliperSlider = sliderObj.AddComponent<CaliperSlider>();

        // Attach readout logic
        measurementReader = readoutObj.AddComponent<MeasurementReader>();
        measurementReader.vernierSlider = sliderObj.transform;
        measurementReader.displayText = readoutText;
        measurementReader.maxMM = 150f;

        // 10. Hook drag callbacks
        caliperSlider.OnDragReleased += OnCaliperDragReleased;

        // 11. Connect Visibility with Toggle in Checklist
        GameObject toggleObj = GameObject.Find("Toggle_VernierPresent ") ?? GameObject.Find("Toggle_VernierPresent");
        if (toggleObj != null)
        {
            Toggle toggleVernier = toggleObj.GetComponent<Toggle>();
            if (toggleVernier != null)
            {
                toggleVernier.onValueChanged.AddListener(isOn => {
                    vernierPanel.SetActive(isOn);
                });
                vernierPanel.SetActive(toggleVernier.isOn);
            }
        }
        else
        {
            Debug.LogWarning("[VernierIntegration] Toggle_VernierPresent not found in scene.");
        }
    }

    private void OnCaliperDragReleased()
    {
        if (measurementReader != null && measurementBridge != null)
        {
            float val = measurementReader.CurrentValue;
            Debug.Log($"[VernierIntegration] Slider drag released. Sending measurement: {val} mm");
            measurementBridge.SendMeasurement(val, "VernierCaliper");
        }
    }

    void Update()
    {
        if (caliperSlider == null || measurementReader == null || startLimitObj == null || endLimitObj == null) return;

        // Read boundaries dynamically in world space coordinates
        float min = startLimitObj.transform.position.x;
        float max = endLimitObj.transform.position.x;

        caliperSlider.minX = min;
        caliperSlider.maxX = max;

        measurementReader.minX = min;
        measurementReader.maxX = max;

        // Live synchronise measurements to the checklist input field while dragging
        if (targetInputField != null && caliperSlider.IsDragging)
        {
            targetInputField.text = measurementReader.CurrentValue.ToString("F1");
        }

        // Play micro-animation on measurement change
        float currentVal = measurementReader.CurrentValue;
        if (currentVal != lastValue)
        {
            lastValue = currentVal;
            StopAllCoroutines();
            StartCoroutine(PulseText());
        }
    }

    private System.Collections.IEnumerator PulseText()
    {
        readoutText.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
        float elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            readoutText.transform.localScale = Vector3.Lerp(new Vector3(1.15f, 1.15f, 1f), Vector3.one, elapsed / 0.15f);
            yield return null;
        }
        readoutText.transform.localScale = Vector3.one;
    }

    // Procedural generation of ruler ticks
    private Texture2D GenerateMainScaleTexture()
    {
        Texture2D tex = new Texture2D(330, 40, TextureFormat.RGBA32, false);
        Color bgColor = new Color(0.08f, 0.09f, 0.14f, 0.97f);  // Deep charcoal
        
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                tex.SetPixel(x, y, bgColor);

        // Draw baseline
        for (int x = 0; x < 300; x++)
            tex.SetPixel(x, 10, new Color(0.05f, 0.75f, 0.55f, 0.7f));  // Emerald baseline

        // Draw scale: 300 pixels span for 150mm (so 1mm = 2px)
        for (int x = 0; x <= 300; x += 2)
        {
            int mmValue = x / 2;
            if (mmValue % 10 == 0) // CM (every 10mm)
            {
                DrawLine(tex, x, 10, 20, new Color(0.85f, 0.90f, 0.95f, 1f), 2);
            }
            else if (mmValue % 5 == 0) // Half-CM (every 5mm)
            {
                DrawLine(tex, x, 10, 14, new Color(0.8f, 0.8f, 0.8f, 1f), 1);
            }
            else // Millimeters (every 1mm)
            {
                DrawLine(tex, x, 10, 9, new Color(0.6f, 0.6f, 0.6f, 0.8f), 1);
            }
        }
        tex.Apply();
        return tex;
    }

    // Procedural generation of Vernier divisions
    private Texture2D GenerateVernierScaleTexture()
    {
        Texture2D tex = new Texture2D(60, 50, TextureFormat.RGBA32, false);
        Color bgColor = new Color(0.04f, 0.14f, 0.10f, 0.95f);  // Dark emerald tint

        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                tex.SetPixel(x, y, bgColor);

        // Draw sliding baseline
        for (int x = 0; x < tex.width; x++)
            tex.SetPixel(x, 40, new Color(0.05f, 0.82f, 0.55f, 0.8f));  // Emerald baseline

        // 10 divisions spanning 9 divisions of main scale (9 * 2px = 18px total span)
        for (int i = 0; i <= 10; i++)
        {
            int x = Mathf.RoundToInt(i * 1.8f);
            if (i == 0 || i == 10)
            {
                DrawLine(tex, x, 25, 15, Color.white, 2);
            }
            else if (i == 5)
            {
                DrawLine(tex, x, 30, 10, new Color(0.9f, 0.9f, 0.9f, 1f), 1);
            }
            else
            {
                DrawLine(tex, x, 33, 7, new Color(0.7f, 0.7f, 0.7f, 0.9f), 1);
            }
        }
        tex.Apply();
        return tex;
    }

    private void DrawLine(Texture2D tex, int x, int yStart, int height, Color color, int thickness = 1)
    {
        for (int y = yStart; y < yStart + height; y++)
        {
            for (int t = 0; t < thickness; t++)
            {
                if (x + t >= 0 && x + t < tex.width && y >= 0 && y < tex.height)
                {
                    tex.SetPixel(x + t, y, color);
                }
            }
        }
    }
}
