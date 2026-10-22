using UnityEngine;
using System.Collections.Generic;

public class RecruitmentManager : MonoBehaviour
{
    public static RecruitmentManager Instance { get; private set; }

    public List<LecturerData> availableLecturers = new List<LecturerData>();

    // Antrean dosen yang udah di-hire tapi belum muncul di map
    private List<LecturerData> pendingHires = new List<LecturerData>();

    private int freeHiresToday = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        GenerateCandidates();
    }

    public void GenerateCandidates()
    {
        availableLecturers.Clear();
        int currentDay = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;

        if (currentDay == 1)
        {
            // HARI 1: Dijamin ada 2 kandidat (Gratis biaya hire)
            availableLecturers.Add(LecturerData.GenerateRandomLecturer(currentDay, false));
            availableLecturers.Add(LecturerData.GenerateRandomLecturer(currentDay, false));
            freeHiresToday = 2;
            Debug.Log("Hari 1: Ada 2 Dosen Gratis untuk direkrut!");
        }
        else
        {
            // HARI BIASA: 0 - 2 kandidat secara random
            int jumlahKandidat = Random.Range(0, 3);
            for (int i = 0; i < jumlahKandidat; i++)
            {
                bool isNightOwl = Random.value > 0.8f; // 20% chance bisa kerja malam
                availableLecturers.Add(LecturerData.GenerateRandomLecturer(currentDay, isNightOwl));
            }
            freeHiresToday = 0;
        }
    }

    public bool TryHireLecturer(LecturerData data)
    {
        // Cek biaya hire (Hari 1 gratis, hari lain bayar 2x gaji)
        int hireCost = (freeHiresToday > 0) ? 0 : (data.salary * 2);

        if (ResourceManager.Instance.SpendMoney(hireCost))
        {
            if (freeHiresToday > 0) freeHiresToday--; // Kurangi kuota gratis

            // Masukkan ke antrean (belum muncul di map)
            pendingHires.Add(data);
            availableLecturers.Remove(data); // Hilang dari daftar pelamar

            Debug.Log($"<color=green>{data.lecturerName} diterima kerja!</color> (Biaya Hire: {hireCost}). Dia akan datang di fase berikutnya.");
            return true;
        }
        else
        {
            Debug.LogWarning("Uang tidak cukup untuk biaya Hire!");
            return false;
        }
    }

    // Fungsi ini dipanggil GameManager setiap ganti fase
    public void ProcessPendingHires()
    {
        if (pendingHires.Count == 0) return;

        // Pindahkan semua antrean ke map
        List<LecturerData> toSpawn = new List<LecturerData>(pendingHires);
        pendingHires.Clear();

        foreach (var data in toSpawn)
        {
            if (GameManager.Instance != null && GameManager.Instance.lecturerPrefab != null)
            {
                GameObject newLecturerObj = Instantiate(GameManager.Instance.lecturerPrefab, Vector2.zero, Quaternion.identity);
                LecturerAI ai = newLecturerObj.GetComponent<LecturerAI>();

                if (ai != null)
                {
                    ai.compliance = data.compliance;
                    ai.auraPerDay = data.auraPerDay;
                    ai.canWorkNight = data.canWorkNight;
                    ai.lecturerName = data.lecturerName; // Simpan nama di AI
                }

                GameManager.Instance.lecturerCount++;
            }
        }
    }
}