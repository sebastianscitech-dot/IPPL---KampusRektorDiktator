using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform rektorTarget;

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.15f;
    private Vector3 cameraVelocity = Vector3.zero;

    [Header("Freecam Settings")]
    [SerializeField] private float freecamSpeed = 10f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 15f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed * 0.1f;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    private void LateUpdate()
    {
        bool isBuildMode = BuildManager.Instance != null && BuildManager.Instance.isBuildMode;

        if (isBuildMode)
        {
            Vector2 moveInput = Vector2.zero;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x += 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x -= 1;

            Vector3 moveDir = new Vector3(moveInput.x, moveInput.y, 0).normalized;
            transform.position += moveDir * freecamSpeed * Time.deltaTime;
        }
        else
        {
            if (rektorTarget != null)
            {
                Vector3 targetPos = new Vector3(rektorTarget.position.x, rektorTarget.position.y, -10f);
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref cameraVelocity, smoothTime);
            }
        }
    }
}