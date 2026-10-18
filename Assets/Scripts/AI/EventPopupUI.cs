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
        // TAMBAHAN: Terapkan dampak sebelum menutup
        if (currentEvent != null)
        {
            if (AuraManager.Instance != null)
                AuraManager.Instance.AddAura(currentEvent.auraImpact);

            if (ResourceManager.Instance != null)
            {
                if (currentEvent.moneyImpact > 0)
                    ResourceManager.Instance.AddMoney(currentEvent.moneyImpact);
                else if (currentEvent.moneyImpact < 0)
                    ResourceManager.Instance.SpendMoney(Mathf.Abs(currentEvent.moneyImpact)); // Ubah minus jadi plus buat biaya
            }

            Debug.Log($"Dampak Diterapkan: Aura {currentEvent.auraImpact}, Uang {currentEvent.moneyImpact}");
            currentEvent = null; // Reset
        }

        panelUtama.SetActive(false);
    }
}