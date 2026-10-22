using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManagePanelUI : MonoBehaviour
{
    [Header("Sub-Panels")]
    public GameObject panelHireDosen;
    public GameObject panelManageDosen;

    [Header("Hire Panel Settings")]
    public Transform contentPelamar;
    public GameObject cardPelamarPrefab;
    public Button btnHireAction; // Tombol Hire di luar scroll
    public TextMeshProUGUI textHireInfo;

    [Header("Manage Panel Settings")]
    public Transform contentDosenAktif;
    public GameObject cardDosenAktifPrefab;
    public Button btnFireAction; // Tombol Fire di luar scroll
    public TextMeshProUGUI textFireInfo;

    // Variabel buat nyimpen data yang lagi dipilih
    private LecturerData selectedCandidate;
    private LecturerAI selectedActiveLecturer;

    // Variabel buat ngubah warna kartu
    private CardSelection selectedCandidateCard;
    private CardSelection selectedActiveCard;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;

        ShowHirePanel(); // Buka panel Hire secara default
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
    }

    private void HandlePhaseChanged(GameManager.GamePhase newPhase)
    {
        // Kasih jeda dikit (0.05 detik) biar objek dosennya beneran selesai di-spawn
        // baru UI-nya di-refresh
        Invoke("RefreshUI", 0.05f);
    }

    public void ShowHirePanel()
    {
        panelHireDosen.SetActive(true);
        panelManageDosen.SetActive(false);
        ClearSelection();
        RefreshUI();
    }

    public void ShowManagePanel()
    {
        panelHireDosen.SetActive(false);
        panelManageDosen.SetActive(true);
        ClearSelection();
        RefreshUI();
    }

    private void ClearSelection()
    {
        selectedCandidate = null;
        selectedActiveLecturer = null;
        selectedCandidateCard = null;
        selectedActiveCard = null;
        UpdateButtons();
    }

    [Header("Manage Panel Extra")]
    public Button btnAppeaseAction;
    public TextMeshProUGUI textAppeaseInfo;

    public void RefreshUI()
    {
        if (RecruitmentManager.Instance == null) return;

        // Refresh Kartu Pelamar (Hire)
        foreach (Transform child in contentPelamar) Destroy(child.gameObject);
        foreach (var data in RecruitmentManager.Instance.availableLecturers)
        {
            GameObject card = Instantiate(cardPelamarPrefab, contentPelamar);
            card.transform.localScale = Vector3.one;

            TMP_Text[] texts = card.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 3)
            {
                texts[0].text = data.lecturerName;
                texts[1].text = $"Gaji: {data.salary}/hari";
                texts[2].text = $"Aura: {data.auraPerDay:F1} {(data.canWorkNight ? "[Malam]" : "")}";
            }

            CardSelection sel = card.GetComponent<CardSelection>();
            if (sel == null) sel = card.AddComponent<CardSelection>();
            sel.lecturerData = data;
            sel.isActiveLecturerCard = false;
            sel.managePanel = this;
        }

        // Refresh Kartu Dosen Aktif (Fire)
        foreach (Transform child in contentDosenAktif) Destroy(child.gameObject);
        LecturerAI[] activeLecturers = FindObjectsByType<LecturerAI>(FindObjectsInactive.Exclude);
        foreach (var dosen in activeLecturers)
        {
            GameObject card = Instantiate(cardDosenAktifPrefab, contentDosenAktif);
            card.transform.localScale = Vector3.one;

            TMP_Text[] texts = card.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = dosen.lecturerName;
                texts[1].text = $"Status: {(dosen.isStriking ? "<color=red>Mogok</color>" : "<color=green>Aktif</color>")}\nAura: {dosen.auraPerDay:F1}";
            }

            CardSelection sel = card.GetComponent<CardSelection>();
            if (sel == null) sel = card.AddComponent<CardSelection>();
            sel.activeLecturer = dosen;
            sel.isActiveLecturerCard = true;
            sel.managePanel = this;
        }

        UpdateButtons();
    }

    // Dipanggil oleh CardSelection
    public void SelectCandidate(LecturerData data, CardSelection card)
    {
        if (selectedCandidateCard != null) selectedCandidateCard.SetSelected(false);
        selectedCandidate = data;
        selectedCandidateCard = card;
        selectedCandidateCard.SetSelected(true);
        UpdateButtons();
    }

    public void SelectActiveLecturer(LecturerAI ai, CardSelection card)
    {
        if (selectedActiveCard != null) selectedActiveCard.SetSelected(false);
        selectedActiveLecturer = ai;
        selectedActiveCard = card;
        selectedActiveCard.SetSelected(true);
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        // Logika Tombol Hire
        if (btnHireAction != null)
        {
            btnHireAction.interactable = (selectedCandidate != null);
            if (textHireInfo != null)
                textHireInfo.text = selectedCandidate != null ? $"Terpilih: {selectedCandidate.lecturerName}" : "Pilih kartu terlebih dahulu";
        }

        // Logika Tombol Fire
        if (btnFireAction != null)
        {
            btnFireAction.interactable = (selectedActiveLecturer != null);
            if (textFireInfo != null)
                textFireInfo.text = selectedActiveLecturer != null ? $"Terpilih: {selectedActiveLecturer.lecturerName}" : "Pilih kartu terlebih dahulu";
        }

        if (btnAppeaseAction != null)
        {
            // Hanya bisa diklik kalau dosennya lagi mogok
            bool isSelectedStriking = (selectedActiveLecturer != null && selectedActiveLecturer.isStriking);
            btnAppeaseAction.interactable = isSelectedStriking;

            if (textAppeaseInfo != null)
                textAppeaseInfo.text = isSelectedStriking ? "Bujuk Dosen (100)" : "Dosen tidak mogok";
        }
    }

    // Fungsi untuk tombol luar
    public void ExecuteHire()
    {
        if (selectedCandidate == null) return;
        if (RecruitmentManager.Instance.TryHireLecturer(selectedCandidate))
        {
            ClearSelection();
            // GANTI RefreshUI() JADI INI:
            Invoke("RefreshUI", 0.05f);
        }
    }

    public void ExecuteFire()
    {
        if (selectedActiveLecturer == null) return;

        Destroy(selectedActiveLecturer.gameObject);
        GameManager.Instance.lecturerCount--;
        ClearSelection();

        // GANTI RefreshUI() JADI INI:
        Invoke("RefreshUI", 0.05f);
    }

    public void ExecuteAppease()
    {
        if (selectedActiveLecturer == null) return;

        // Biaya bujuk = 2x gaji harian
        int appeaseCost = 100; // Bisa kamu ubah jadi dinamis

        if (ResourceManager.Instance.SpendMoney(appeaseCost))
        {
            selectedActiveLecturer.Appease(50f); // Naikkan kepatuhan 50 poin
            ClearSelection();
            Invoke("RefreshUI", 0.05f);
        }
        else
        {
            Debug.LogWarning("Uang tidak cukup untuk membujuk dosen!");
        }
    }
}