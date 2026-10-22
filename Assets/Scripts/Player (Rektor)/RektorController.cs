using UnityEngine;
using UnityEngine.InputSystem;

public class RektorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsEventActive)
        {
            movement = Vector2.zero; // TAMBAHKAN BARIS INI (Rem mendadak)
            return;
        }

        if (BuildManager.Instance != null && BuildManager.Instance.isBuildMode)
        {
            movement = Vector2.zero;
            return;
        }

        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x += 1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x -= 1;

        movement = moveInput;
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = movement.normalized * moveSpeed;
        
        // Paksa berhenti kalau Build Mode ATAU Event aktif
        if ((BuildManager.Instance != null && BuildManager.Instance.isBuildMode) || 
            (GameManager.Instance != null && GameManager.Instance.IsEventActive))
        {
            targetVelocity = Vector2.zero;
        }

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }
}