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
    public GameObject pauseMenuPanel;

    [Header("Control Panel")]
    public TextMeshProUGUI textBuildStatus;
    public Color normalColor = Color.white;
    public Color activeBuildColor = Color.red;

    [Header("Pause Button")]
    public TextMeshProUGUI textPauseButton;

    [Header("Player Info")]
    public TextMeshProUGUI textNamaRektor;

    public TextMeshProUGUI textEksposur;
    public TextMeshProUGUI textDebtWarning;

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
        if (textNamaRektor != null)
        {
            textNamaRektor.text = $"Rektor: {PlayerPrefs.GetString("RektorName", "Rektor Abadi")}";
        }

        if (ExposureManager.Instance != null)
        {
            ExposureManager.Instance.OnExposureChanged += UpdateExposureUI;
            UpdateExposureUI(ExposureManager.Instance.Exposure); // Set awal
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

    private void UpdateExposureUI(float value)
    {
        if (textEksposur != null)
            textEksposur.text = $"Eksposur : {value:F1}%"; // Format 1 desimal
    }

    // --- Fungsi Update UI ---

    public void UpdateResourceUI()
    {
        if (ResourceManager.Instance == null) return;

        int money = ResourceManager.Instance.Money;
        textUang.text = $"Uang : {money}";

        // TAMBAHKAN INI: Kalau minus, teks jadi merah
        textUang.color = (money < 0) ? Color.red : Color.white;

        textManusia.text = $"Manusia : {ResourceManager.Instance.HumanCount}";

        if (textDebtWarning != null && GameManager.Instance != null)
        {
            if (GameManager.Instance.daysInDebt > 0)
            {
                textDebtWarning.gameObject.SetActive(true);
                int sisaHari = 3 - GameManager.Instance.daysInDebt;
                textDebtWarning.text = $"<color=red>PERINGATAN BANGKRUT: {sisaHari} HARI LAGI!</color>";
            }
            else
            {
                textDebtWarning.gameObject.SetActive(false);
            }
        }
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

    private void UpdatePauseUI(bool isPaused)
    {
        if (textPauseButton != null)
            textPauseButton.text = isPaused ? "▶" : "II";

        // TAMBAHAN: Munculkan/Sembunyikan panel menu
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(isPaused);
    }

    // Fungsi untuk tombol "X" di Resources (opsional)
    public void ToggleResourcesPanel()
    {
        if (resourcesPanel != null)
            resourcesPanel.SetActive(!resourcesPanel.activeSelf);
    }
}