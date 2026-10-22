using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get; private set; }
    public int jumlahSuapInv = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // 1. BELI IKLAN
    public void BuyAdsAction()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (GameManager.Instance != null) GameManager.Instance.BuyAds();
    }

    // 2. PECAT DOSEN
    public void FireLecturerAction()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        LecturerAI[] lecturers = FindObjectsByType<LecturerAI>(FindObjectsInactive.Exclude);
        if (lecturers.Length > 0)
        {
            Destroy(lecturers[0].gameObject);
            GameManager.Instance.lecturerCount--; // Kurangi jumlah dosen di GameManager
            Debug.Log("<color=orange>Dosen dipecat!</color> Gaji berkurang.");
        }
        else
        {
            Debug.LogWarning("Tidak ada dosen yang bisa dipecat!");
        }
    }

    // 3. PECAT STAF
    public void FireStaffAction()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        StaffAI[] staffs = FindObjectsByType<StaffAI>(FindObjectsInactive.Exclude);
        if (staffs.Length > 0)
        {
            Destroy(staffs[0].gameObject);
            GameManager.Instance.staffCount--; // Kurangi jumlah staf di GameManager
            Debug.Log("<color=orange>Staf dipecat!</color> Gaji berkurang.");
        }
        else
        {
            Debug.LogWarning("Tidak ada staf yang bisa dipecat!");
        }
    }

    // 4A. USIR INVESTIGATOR (BAYAR)
    public int GetBribeCost()
    {
        // Rumus dari kamu: 150 * (jumlahSuapInv * 0.5)
        float calculatedCost = 150f * (jumlahSuapInv * 0.5f);

        // Bulatkan ke atas (Ceil) dan pastikan minimal 150
        int finalCost = Mathf.CeilToInt(calculatedCost);
        return Mathf.Max(150, finalCost);
    }

    public void RemoveInvestigatorPayAction()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        int bribeCost = GetBribeCost(); // Panggil harga dinamis
        InvestigatorAI[] investigators = FindObjectsByType<InvestigatorAI>(FindObjectsInactive.Exclude);

        if (investigators.Length > 0)
        {
            if (ResourceManager.Instance.SpendMoney(bribeCost))
            {
                Destroy(investigators[0].gameObject);
                jumlahSuapInv += 1; // Tambah jumlah suap berhasil
                Debug.Log($"<color=green>Investigator disuap sebesar {bribeCost}!</color>");
            }
            else
            {
                Debug.LogWarning($"Uang tidak cukup! Butuh {bribeCost} untuk menyuap.");
            }
        }
        else
        {
            Debug.LogWarning("Tidak ada Investigator untuk disuap.");
        }
    }

    // 4B. USIR INVESTIGATOR (GRATIS)
    public void RemoveInvestigatorFreeAction()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        InvestigatorAI[] investigators = FindObjectsByType<InvestigatorAI>(FindObjectsInactive.Exclude);

        if (investigators.Length > 0)
        {
            Destroy(investigators[0].gameObject);

            // 50% chance nambah penalti permanen
            if (Random.value <= 0.5f)
            {
                WorldDirectorAI.Instance.investigatorSpawnBonus++;
                Debug.LogError("<color=red>INVESTIGATOR MARAH!</color> Kamu mengusirnya dengan kasar. Investigator berikutnya bertambah 1 secara permanen!");
            }
            else
            {
                Debug.Log("<color=green>Investigator pergi tanpa masalah.</color> Kamu beruntung.");
            }
        }
        else
        {
            Debug.LogWarning("Tidak ada Investigator untuk diusir.");
        }
    }
}