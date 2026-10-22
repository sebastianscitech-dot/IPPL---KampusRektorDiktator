using UnityEngine;
using UnityEngine.InputSystem;

public class ManagePanelManager : MonoBehaviour
{
    public GameObject managePanel;
    private bool isPanelOpen = false;

    private void Start()
    {
        if (managePanel != null) managePanel.SetActive(false);
    }

    private void Update()
    {
        // Jangan buka kalau game pause, event, atau game over
        if (GameManager.Instance != null &&
           (GameManager.Instance.IsEventActive || GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver))
            return;

        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (managePanel != null)
        {
            isPanelOpen = !isPanelOpen;
            managePanel.SetActive(isPanelOpen);
        }
    }

    public void ClosePanel()
    {
        if (managePanel != null)
        {
            isPanelOpen = false;
            managePanel.SetActive(false);
        }
    }
}