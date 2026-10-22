using UnityEngine;

public class DirtyActionManager : MonoBehaviour
{
    public static DirtyActionManager Instance { get; private set; }

    [Header("Biaya Kubur Bukti")]
    public int moneyCost = 100;
    public int humanCost = 2;

    [Header("Efek Kubur Bukti")]
    public float exposureReduction = 30f;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void BuryEvidence()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (ResourceManager.Instance == null || ExposureManager.Instance == null) return;

        if (ResourceManager.Instance.Money >= moneyCost && ResourceManager.Instance.HumanCount >= humanCost)
        {
            // 1. Potong Resource
            ResourceManager.Instance.SpendMoney(moneyCost);
            ResourceManager.Instance.RemoveHuman(humanCost);

            // 2. Turunkan Eksposur
            ExposureManager.Instance.ReduceExposure(exposureReduction);

            Debug.Log($"<color=green>BUKTI DIKUBUR!</color> Eksposur turun {exposureReduction}%. Uang -{moneyCost}, Manusia -{humanCost}.");
        }
        else
        {
            Debug.LogWarning("Gagal Kubur Bukti: Uang atau Manusia tidak cukup!");
        }
    }
}