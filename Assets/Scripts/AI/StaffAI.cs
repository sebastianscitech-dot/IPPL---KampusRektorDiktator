using UnityEngine;

public class StaffAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private void Update()
    {
        if (BuildManager.Instance != null && BuildManager.Instance.isBuildMode) return;
        if (GameManager.Instance != null && GameManager.Instance.IsEventActive) return;

        // Jalan mondar-mandir
        transform.position += (Vector3)(Vector2.up * Mathf.Sin(Time.time * 3f) * moveSpeed * Time.deltaTime);
    }
}