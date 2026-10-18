using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Top Bar")]
    public TextMeshProUGUI textEvent;
    public TextMeshProUGUI textHariWaktu;

    [Header("Resources")]
    public TextMeshProUGUI textUang;
    public TextMeshProUGUI textManusia;
    public TextMeshProUGUI textAura;
    public GameObject resourcesPanel;

    [Header("Control Panel")]
    public TextMeshProUGUI textBuildStatus;
    public Color normalColor = Color.white;
    public Color activeBuildColor = Color.red;

    [Header("Pause Button")]
    public TextMeshProUGUI textPauseButton;

    private void Start()
    {
        // Daftarkan event listener
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateResourceUI;
            UpdateResourceUI();
        }
        if (AuraManager.Instance != null)
        {
            AuraManager.Instance.OnAuraChanged += UpdateAuraUI;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhaseChanged += UpdateTimeUI;
            GameManager.Instance.OnPauseToggled += UpdatePauseUI; // Listener Pause
            UpdateTimeUI(GameManager.Instance.CurrentPhase);
        }
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.OnBuildModeToggled += UpdateBuildModeUI; // Listener Build
        }
    }

    private void OnDisable()
    {
        // Bersihkan listener
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnResourceChanged -= UpdateResourceUI;
        if (AuraManager.Instance != null)
            AuraManager.Instance.OnAuraChanged -= UpdateAuraUI;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhaseChanged -= UpdateTimeUI;
            GameManager.Instance.OnPauseToggled -= UpdatePauseUI;
        }
        if (BuildManager.Instance != null)
            BuildManager.Instance.OnBuildModeToggled -= UpdateBuildModeUI;
    }

    // --- Fungsi Update UI ---

    public void UpdateResourceUI()
    {
        if (ResourceManager.Instance == null) return;
        textUang.text = $"Uang : {ResourceManager.Instance.Money}";
        textManusia.text = $"Manusia : {ResourceManager.Instance.HumanCount}";
    }

    public void UpdateAuraUI(float auraValue, AuraManager.AuraCategory category)
    {
        textAura.text = $"Aura : {category} | {auraValue}";
    }

    public void UpdateTimeUI(GameManager.GamePhase phase)
    {
        textHariWaktu.text = $"Hari {GameManager.Instance.CurrentDay} | {phase}";
    }

    public void UpdateBuildModeUI(bool isActive)
    {
        if (textBuildStatus != null)
        {
            textBuildStatus.color = isActive ? activeBuildColor : normalColor;
        }
    }

    public void UpdatePauseUI(bool isPaused)
    {
        if (textPauseButton != null)
        {
            textPauseButton.text = isPaused ? "▶" : "II";
        }
    }

    // Fungsi untuk tombol "X" di Resources (opsional)
    public void ToggleResourcesPanel()
    {
        if (resourcesPanel != null)
            resourcesPanel.SetActive(!resourcesPanel.activeSelf);
    }
}