using UnityEngine;

public class StudentAI : MonoBehaviour
{
    private enum State { Idle, Walk }
    private State currentState;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float walkDuration = 3f;

    private float stateTimer;
    private Vector2 targetPosition;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cek kalau Rigidbody-nya nggak ada
        if (rb == null)
        {
            Debug.LogError($"Woy! {gameObject.name} nggak punya Rigidbody2D! Tambahin dong!");
        }
    }

    private void Start()
    {
        ChangeState(State.Idle);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsEventActive) return;

        stateTimer -= Time.deltaTime;
        if (BuildManager.Instance != null && BuildManager.Instance.isBuildMode)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return; // Skip sisa kode di bawahnya
        }
        switch (currentState)
        {
            case State.Idle:
                if (stateTimer <= 0)
                {
                    Debug.Log($"{gameObject.name} mulai jalan.");
                    ChangeState(State.Walk);
                }
                break;

            case State.Walk:
                MoveTowardsTarget();
                if (stateTimer <= 0)
                {
                    Debug.Log($"{gameObject.name} berhenti.");
                    ChangeState(State.Idle);
                }
                break;
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        if (currentState == State.Idle)
        {
            stateTimer = idleDuration;
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
        else if (currentState == State.Walk)
        {
            stateTimer = walkDuration;
            targetPosition = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-3f, 3f), UnityEngine.Random.Range(-3f, 3f));
            Debug.Log($"Target baru: {targetPosition}");
        }
    }

    private void MoveTowardsTarget()
    {
        // Ganti rb.linearVelocity dengan ini:
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }
}