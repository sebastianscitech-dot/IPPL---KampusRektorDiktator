using UnityEngine;

public class RektorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float acceleration = 15f;

    [Header("Zoom Settings")] // TAMBAHAN BARU
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector3 cameraVelocity = Vector3.zero;
    private Camera mainCam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // TAMBAHAN BARU: Logika Scroll Wheel untuk Zoom
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0 && mainCam != null)
        {
            mainCam.orthographicSize -= scroll * zoomSpeed;
            // Batasi zoom biar nggak terlalu dekat/jauh
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, minZoom, maxZoom);
        }
    }

    private void FixedUpdate()
    {
        // Pergerakan pakai Velocity + Akselerasi (biar halus kayak mentega)
        Vector2 targetVelocity = movement.normalized * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (mainCam != null)
        {
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, -10f);
            mainCam.transform.position = Vector3.SmoothDamp(mainCam.transform.position, targetPos, ref cameraVelocity, smoothTime);
        }
    }
}