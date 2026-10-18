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

    public bool IsPaused { get; private set; } = false;
    public event Action<bool> OnPauseToggled;

    public int CurrentDay { get; private set; } = 1;
    private bool isGameOver = false;

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
        if (isGameOver) return;
        // Cek Game Over sederhana
        if (ResourceManager.Instance != null && ResourceManager.Instance.Money <= 0)
            Debug.Log("BANGKRUT!");

        if (ExposureManager.Instance != null && ExposureManager.Instance.Exposure >= 100f)
            Debug.Log("DIPENJARA!");
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        // 0 = berhenti total, 1 = jalan normal
        Time.timeScale = IsPaused ? 0f : 1f;
        OnPauseToggled?.Invoke(IsPaused);
    }

    public void SetPhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;
        OnPhaseChanged?.Invoke(newPhase);

        if (newPhase == GamePhase.Pagi)
        {
            SpawnStudents();
        }

        if (newPhase == GamePhase.Siang)
        {
            TriggerRandomEvent();
            ProcessEconomics();
        }
    }

    private void SpawnStudents()
    {
        if (studentPrefab == null) return;

        for (int i = 0; i < 3; i++)
        {
            Vector2 spawnPos = new Vector2(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
            Instantiate(studentPrefab, spawnPos, Quaternion.identity);

            if (ResourceManager.Instance != null) ResourceManager.Instance.AddHuman(1);
        }
        Debug.Log("Mahasiswa telah datang ke kampus!");
    }

    public void DebugNextPhase()
    {
        NextPhase();
    }

    private void TriggerRandomEvent()
    {
        if (worldDirectorAI != null)
        {
            GameEvent pickedEvent = worldDirectorAI.PickRandomEvent();
            if (pickedEvent != null)
            {
                Debug.Log($"EVENT MUNCUL: [{pickedEvent.category}] {pickedEvent.eventTitle} - {pickedEvent.description}");
   
                if (eventPopupUI != null)
                {
                    eventPopupUI.ShowEvent(pickedEvent);
                }
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
        switch (CurrentPhase)
        {
            case GamePhase.Starting: SetPhase(GamePhase.Pagi); break;
            case GamePhase.Pagi: SetPhase(GamePhase.Siang); break;
            case GamePhase.Siang: SetPhase(GamePhase.Malam); break;
            case GamePhase.Malam:
                CurrentDay++;
                SetPhase(GamePhase.Starting);
                break;
        }
    }
}