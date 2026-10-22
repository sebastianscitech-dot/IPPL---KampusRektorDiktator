using UnityEngine;
using TMPro; // Wajib pakai TMPro

public class ActionPanelStats : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI textAdsInfo;
    public TextMeshProUGUI textLectureInfo;
    public TextMeshProUGUI textStaffInfo;
    public TextMeshProUGUI textInvInfo;

    // Update dipanggil setiap frame selama panel aktif
    private void OnEnable()
    {
        UpdateUI(); // Update sekali saat panel dibuka
    }

    private void Update()
    {
        UpdateUI(); // Biar angkanya selalu update real-time
    }

    private void UpdateUI()
    {
        if (GameManager.Instance == null || ActionManager.Instance == null) return;

        // 1. INFO IKLAN
        if (textAdsInfo != null)
        {
            textAdsInfo.text =  $"Iklan Dibeli: {GameManager.Instance.totalAds} \n" +
                                $" Harga: {GameManager.Instance.adsCost}";
        }

        // 2. INFO DOSEN
        if (textLectureInfo != null)
        {
            int dosenCount = GameManager.Instance.lecturerCount;
            textLectureInfo.text = $"Dosen: {dosenCount} ";
        }

        // 3. INFO STAFF
        if (textStaffInfo != null)
        {
            int stafCount = GameManager.Instance.staffCount;
            textStaffInfo.text = $" Staf: {stafCount}";
        }

        // 4. INFO INVESTIGATOR
        if (textInvInfo != null)
        {
            // Hitung berapa banyak investigator aktif di scene
            InvestigatorAI[] activeInvs = FindObjectsByType<InvestigatorAI>(FindObjectsInactive.Exclude);
            int activeCount = activeInvs.Length;
            int currentBribeCost = ActionManager.Instance.GetBribeCost();

            int nextSpawnCount = 1;
            if (WorldDirectorAI.Instance != null)
            {
                nextSpawnCount += WorldDirectorAI.Instance.investigatorSpawnBonus;
            }

            textInvInfo.text = $"Investigator: {activeCount}\n" +
                               $"Suap: {currentBribeCost} (Total: {ActionManager.Instance.jumlahSuapInv})\n" +
                               $"<color=red>Datang Berikutnya: {nextSpawnCount}</color>";
        }
    }
}