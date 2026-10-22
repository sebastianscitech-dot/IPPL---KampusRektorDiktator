using UnityEngine;
using UnityEngine.InputSystem;

public class ActionPanelManager : MonoBehaviour
{
    public GameObject actionPanel; // Tempat naruh Panel UI-nya
    private bool isPanelOpen = false;

    private void Start()
    {
        if (actionPanel != null) actionPanel.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
                   (GameManager.Instance.IsEventActive || GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver))
            return;

        // Baca input Q
        if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (actionPanel != null)
        {
            isPanelOpen = !isPanelOpen;
            actionPanel.SetActive(isPanelOpen);
        }
    }

    public void ClosePanel()
    {
        if (actionPanel != null)
        {
            isPanelOpen = false;
            actionPanel.SetActive(false);
        }
    }
}