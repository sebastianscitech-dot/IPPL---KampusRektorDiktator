using UnityEngine;

[CreateAssetMenu(fileName = "NewEvent", menuName = "ITK/Game Event")]
public class GameEvent : ScriptableObject
{
    public string eventTitle;
    [TextArea] public string description;

    public enum EventCategory { Positif, Negatif, Aneh }
    public EventCategory category;
    public float baseWeight = 1f;

    [Header("Dampak Event (Impact)")] // TAMBAHAN BARU
    public float auraImpact;    // Contoh: +10 atau -15
    public int moneyImpact;     // Contoh: +50 (untung) atau -30 (rugi)
}