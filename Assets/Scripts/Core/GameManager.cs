using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject studentPrefab;

    [Header("Managers")]
    public WorldDirectorAI worldDirectorAI;

    public EventPopupUI eventPopupUI;

    public static GameManager Instance { get; private set; }

    public enum GamePhase { Starting, Pagi, Siang, Malam }
    public GamePhase CurrentPhase { get; private set; }

    public GameObject gameOverPanel;
    public bool IsPaused { get; private set; } = false;
    public event Action<bool> OnPauseToggled;

    public bool IsEventActive { get; private set; } = false;
    private bool isLoading = false; // Penanda kalau lagi loading

    public int daysInDebt { get; private set; } = 0;
    public int CurrentDay { get; private set; } = 1;
    public bool IsGameOver { get; private set; } = false;

    public TMPro.TextMeshProUGUI textGameOverReason; //

    [Header("Staff Prefabs")]
    public GameObject lecturerPrefab;
    public GameObject staffPrefab;

    [Header("Gaji Harian")]
    public int salaryPerLecturer = 30;
    public int salaryPerStaff = 20;

    public int lecturerCount = 0;
    public int staffCount = 0;

    [Header("Ads System (Iklan)")]
    public int studentSpawnAmount = 3;
    public int adsCost = 50;
    public int totalAds = 0;

    public event Action<GamePhase> OnPhaseChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        SetPhase(GamePhase.Starting);
    }

    private void Update()
    {
        if (IsGameOver) return;

        // if (ResourceManager.Instance.Money <= 0)
        //    TriggerGameOver("BANGKRUT: Utang menumpuk tak terbayar!");

        // TAMBAHAN: Cek Eksposur
        if (ExposureManager.Instance != null && ExposureManager.Instance.Exposure >= 100f)
        {
            TriggerGameOver("DIPENJARA: Eksposur mencapai 100%!");
        }
    }

    public void TogglePause()
    {
        if (IsGameOver) return;

        IsPaused = !IsPaused;
        // 0 = berhenti total, 1 = jalan normal
        Time.timeScale = IsPaused ? 0f : 1f;
        OnPauseToggled?.Invoke(IsPaused);
    }

    public void SetPhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;
        OnPhaseChanged?.Invoke(newPhase);

        if (RecruitmentManager.Instance != null)
            RecruitmentManager.Instance.ProcessPendingHires();

        if (newPhase == GamePhase.Pagi)
        {
            SpawnStudents();
            // SpawnStaff();
            // TriggerLecturerWork();
            GenerateNewCandidates();
        }

        if (newPhase == GamePhase.Siang)
        {
            if (!isLoading)
            {
                TriggerRandomEvent();
                ProcessEconomics();
            }
            TriggerLecturerWork(false); // Dosen ngajar Siang
        }

        if (newPhase == GamePhase.Malam)
        {
            PaySalaries();
            TriggerLecturerWork(true); // Dosen Night Owl ngajar Malam
        }
    }
    private void GenerateNewCandidates()
    {
        if (RecruitmentManager.Instance != null)
            RecruitmentManager.Instance.GenerateCandidates();
    }
    private void SpawnStudents()
    {
        if (studentPrefab == null) return;

        for (int i = 0; i < studentSpawnAmount; i++) // Ganti angka 3 jadi studentSpawnAmount
        {
            Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
            Instantiate(studentPrefab, spawnPos, Quaternion.identity);
            if (ResourceManager.Instance != null) ResourceManager.Instance.AddHuman(1);
        }
    }

    private void SpawnStaff()
    {
        // Buat nyoba, tiap pagi spawn 1 dosen dan 1 pengurus kalau belum ada
        if (lecturerCount < 2 && lecturerPrefab != null)
        {
            Instantiate(lecturerPrefab, new Vector2(-3, 2), Quaternion.identity);
            lecturerCount++;
        }

        if (staffCount < 2 && staffPrefab != null)
        {
            Instantiate(staffPrefab, new Vector2(3, 2), Quaternion.identity);
            staffCount++;
        }
    }

    private void TriggerLecturerWork(bool isNight)
    {
        LecturerAI[] lecturers = FindObjectsByType<LecturerAI>(FindObjectsInactive.Exclude);
        foreach (var dosen in lecturers)
        {
            dosen.DoDailyWork(isNight);
        }
    }

    private void PaySalaries()
    {
        int totalSalary = (lecturerCount * salaryPerLecturer) + (staffCount * salaryPerStaff);

        if (totalSalary > 0)
        {
            ResourceManager.Instance.ForceSpendMoney(totalSalary);
            Debug.Log($"<color=yellow>Gaji Dipotong: -{totalSalary}</color>");

            if (ResourceManager.Instance.Money < 0)
            {
                Debug.LogError("GAJI TIDAK TERCUKUPI! Dosen dan Staf murka!");

                // Hukum semua dosen (Kurangi Kepatuhan)
                LecturerAI[] lecturers = FindObjectsByType<LecturerAI>(FindObjectsInactive.Exclude);
                foreach (var dosen in lecturers)
                {
                    dosen.ReduceCompliance(40f);
                }
            }
            else
            {
                Debug.Log("Gaji Berhasil Dibayar. Karyawan senang.");
            }

            if (ResourceManager.Instance.Money >= totalSalary)
            {
                ResourceManager.Instance.SpendMoney(totalSalary);
                Debug.Log("Gaji Berhasil Dibayar. Karyawan senang.");
            }
            else
            {
                // KONDISI GAJI TIDAK TERCUKUPI
                Debug.LogError("GAJI TIDAK TERCUKUPI! Dosen dan Staf murka!");

                // 1. Uang dipaksa jadi 0
                int sisaUang = ResourceManager.Instance.Money;
                ResourceManager.Instance.SpendMoney(sisaUang);

                // 2. Hukum semua dosen (Kurangi Kepatuhan)
                LecturerAI[] lecturers = FindObjectsByType<LecturerAI>(FindObjectsInactive.Exclude);
                foreach (var dosen in lecturers)
                {
                    dosen.ReduceCompliance(40f); // Turun 40 poin tiap nunggak gaji
                }
            }
        }
    }


    public void DebugNextPhase()
    {
        NextPhase();
    }

    public void EndEvent()
    {
        IsEventActive = false;
    }

    private void TriggerRandomEvent()
    {
        if (worldDirectorAI != null)
        {
            // Untuk testing, kita paksa spawn Investigator setiap kali event Siang
            worldDirectorAI.SpawnInvestigator();

            GameEvent pickedEvent = worldDirectorAI.PickRandomEvent();
            if (pickedEvent != null)
            {
                IsEventActive = true;
                if (eventPopupUI != null) eventPopupUI.ShowEvent(pickedEvent);
            }
        }
    }

    private void ProcessEconomics()
    {
        if (ResourceManager.Instance == null) return;

        // Asumsi: 1 Mahasiswa bayar UKT 50 per hari
        int uktPerMahasiswa = 50;
        int totalUKT = ResourceManager.Instance.HumanCount * uktPerMahasiswa;

        if (totalUKT > 0)
        {
            ResourceManager.Instance.AddMoney(totalUKT);
            Debug.Log($"[Ekonomi] Menerima UKT sebesar {totalUKT} dari {ResourceManager.Instance.HumanCount} mahasiswa.");
        }
        else
        {
            Debug.Log("[Ekonomi] Tidak ada mahasiswa, tidak ada UKT masuk.");
        }

        // Nanti di sini kita bisa kurangi uang untuk gaji Dosen & Pengurus
    }

    public void NextPhase()
    {
        if (IsEventActive || IsGameOver) return;

        switch (CurrentPhase)
        {
            case GamePhase.Starting: SetPhase(GamePhase.Pagi); break;
            case GamePhase.Pagi: SetPhase(GamePhase.Siang); break;
            case GamePhase.Siang: SetPhase(GamePhase.Malam); break;
            case GamePhase.Malam:
                CurrentDay++;
                ProcessDailyDebt(); // TAMBAHKAN INI: Cek utang tiap ganti hari
                if (!IsGameOver) SetPhase(GamePhase.Starting); // Cegah ganti fase kalau udah mati
                break;
        }
    }

    private void ProcessDailyDebt()
    {
        if (ResourceManager.Instance.Money <= 0)
        {
            daysInDebt++;
            Debug.LogWarning($"<color=orange>PERINGATAN!</color> Kampus dalam bahaya bangkrut. Sisa waktu: {3 - daysInDebt} hari lagi!");

            if (daysInDebt >= 3)
            {
                TriggerGameOver("BANGKRUT: Kampus tidak bisa membayar utang selama 3 hari!");
            }
        }
        else
        {
            // Kalau uang positif, reset timer
            if (daysInDebt > 0)
            {
                Debug.Log("<color=green>Utang lunas! Peringatan bangkrut dibatalkan.</color>");
            }
            daysInDebt = 0;
        }
    }

    public void BuyAds()
    {
        if (ResourceManager.Instance.SpendMoney(adsCost))
        {
            studentSpawnAmount++;
            totalAds += 1;
            adsCost += 50 * totalAds;
            Debug.Log($"<color=cyan>Iklan dibeli!</color> Mahasiswa per hari jadi {studentSpawnAmount}. Harga iklan selanjutnya: {adsCost}");
        }
        else
        {
            Debug.LogWarning("Uang tidak cukup buat beli iklan!");
        }
    }

    public void SetData(int savedDay, string savedPhase)
    {
        isLoading = true;
        CurrentDay = savedDay;
        if (System.Enum.TryParse(savedPhase, out GamePhase phase))
        {
            SetPhase(phase);
        }
        isLoading = false;
    }

    private void TriggerGameOver(string reason)
    {
        IsGameOver = true;
        IsEventActive = false; // Matikan event biar gak nyangkut
        Debug.LogError($"[GAME OVER] {reason}");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // TAMBAHKAN INI: Ubah teks penyebab kematian
            if (textGameOverReason != null) textGameOverReason.text = reason;
        }

        Time.timeScale = 0f;
    }
}