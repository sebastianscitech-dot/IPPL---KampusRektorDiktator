using UnityEngine;

public class InvestigatorAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D rb;
    private bool isInvestigating = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Saat muncul, dia langsung mendaftar ke ExposureManager
        if (ExposureManager.Instance != null)
        {
            ExposureManager.Instance.RegisterInvestigator();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null &&
          (GameManager.Instance.IsEventActive || GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!isInvestigating)
        {
            // Jalan ke arah tengah kampus (0,0)
            Vector2 direction = (Vector2.zero - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            // Kalau udah deket tengah, berhenti dan mulai selidiki
            if (Vector2.Distance(transform.position, Vector2.zero) < 1f)
            {
                isInvestigating = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    // Fungsi ini dipanggil saat investigator dihapus/diusir
    private void OnDestroy()
    {
        if (ExposureManager.Instance != null)
        {
            ExposureManager.Instance.UnregisterInvestigator();
        }
    }
}