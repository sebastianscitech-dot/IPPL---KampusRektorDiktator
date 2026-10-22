using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSelection : MonoBehaviour, IPointerClickHandler
{
    public bool isActiveLecturerCard = false; // TAMBAHKAN INI

    public LecturerData lecturerData;
    public LecturerAI activeLecturer;
    public ManagePanelUI managePanel;

    private Image background;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private void Awake()
    {
        background = GetComponent<Image>();
        if (background != null) normalColor = background.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // PAKAI BOOLEAN, BUKAN NULL CHECK
        if (isActiveLecturerCard)
        {
            managePanel.SelectActiveLecturer(activeLecturer, this);
        }
        else
        {
            managePanel.SelectCandidate(lecturerData, this);
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (background != null)
            background.color = isSelected ? selectedColor : normalColor;
    }
}