using UnityEngine;

public class LecturerAI : MonoBehaviour
{
    [Header("Stats")]
    public float compliance = 100f; // 0 - 100
    public float auraPerDay = 2f;
    public float moveSpeed = 1.5f;

    [Header("State")]
    public bool isStriking = false; // Sedang mogok atau tidak
    private SpriteRenderer sr;
    private Color originalColor;

    public string lecturerName = "Dosen Anonim";
    public bool canWorkNight = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color; // Simpan warna asli (Kuning)
    }

    private void Update()
    {
        if (BuildManager.Instance != null && BuildManager.Instance.isBuildMode) return;
        if (GameManager.Instance != null && GameManager.Instance.IsEventActive) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // Kalau mogok, dia diam aja
        if (isStriking) return;

        // Jalan santai acak
        transform.position += (Vector3)(Vector2.right * Mathf.Sin(Time.time * 2f) * moveSpeed * Time.deltaTime);
    }

    // Fungsi yang dipanggil GameManager tiap hari
    public void DoDailyWork(bool isNightShift)
    {
        if (isNightShift && !canWorkNight) return; // Kalau nggak bisa malam
        if (isStriking)
        {
            Debug.LogWarning($"{lecturerName} mogok! Tidak menghasilkan Aura.");
            return;
        }

        if (AuraManager.Instance != null)
        {
            AuraManager.Instance.AddAura(auraPerDay);
            Debug.Log($"<color=cyan>{lecturerName} mengajar {(isNightShift ? "Malam" : "Siang")}!</color> Aura +{auraPerDay}");
        }
    }

    // Fungsi untuk mengurangi kepatuhan
    public void ReduceCompliance(float amount)
    {
        compliance -= amount;
        Debug.Log($"Kepatuhan Dosen turun menjadi {compliance}");

        if (compliance <= 30f && !isStriking)
        {
            StartStrike();
        }
    }

    public void Appease(float complianceBoost)
    {
        compliance += complianceBoost;

        if (compliance > 30f)
        {
            isStriking = false;
            if (sr != null) sr.color = originalColor; // Balik ke warna kuning
            Debug.Log($"<color=green>{lecturerName} BERHENTI MOGOK!</color> Kepatuhan naik jadi {compliance}");
        }
        else
        {
            Debug.Log($"{lecturerName} masih kesal. Kepatuhan sekarang: {compliance}");
        }
    }

    private void StartStrike()
    {
        isStriking = true;
        if (sr != null) sr.color = Color.red; // Ubah warna jadi merah tanda ngamuk
        Debug.LogError($"<color=red>DOSEN MOGOK!</color> {gameObject.name} menolak mengajar!");

        // Opsional: Kalau compliance 0, dia kabur
        if (compliance <= 0)
        {
            Debug.Log($"Dosen {gameObject.name} KABUR dari kampus!");
            Destroy(gameObject); // Hancurkan objek
            if (GameManager.Instance != null) GameManager.Instance.lecturerCount--;
        }
    }
}