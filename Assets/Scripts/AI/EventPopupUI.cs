using UnityEngine;
using TMPro;

public class EventPopupUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI textJudul;
    public TextMeshProUGUI textDeskripsi;
    public GameObject panelUtama;

    private GameEvent currentEvent; // Simpan event yang sedang aktif

    public void ShowEvent(GameEvent evt)
    {
        currentEvent = evt; // Simpan datanya
        panelUtama.SetActive(true);
        textJudul.text = evt.eventTitle;
        textDeskripsi.text = evt.description;
    }

    public void ClosePopup()
    {
        if (currentEvent != null)
        {
            if (AuraManager.Instance != null) AuraManager.Instance.AddAura(currentEvent.auraImpact);
            if (ResourceManager.Instance != null)
            {
                if (currentEvent.moneyImpact > 0) ResourceManager.Instance.AddMoney(currentEvent.moneyImpact);
                else if (currentEvent.moneyImpact < 0) ResourceManager.Instance.SpendMoney(Mathf.Abs(currentEvent.moneyImpact));
            }
            currentEvent = null;
        }

        panelUtama.SetActive(false);

        // kunci sampai ditutup
        if (GameManager.Instance != null) GameManager.Instance.EndEvent();
    }
}