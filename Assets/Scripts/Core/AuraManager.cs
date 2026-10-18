using UnityEngine;
using System;

public class AuraManager : MonoBehaviour
{
    public static AuraManager Instance { get; private set; }

    [SerializeField] private float currentAura = 0f;
    public float Aura => currentAura;

    public enum AuraCategory { Terang, Netral, Gelap }
    public AuraCategory Category { get; private set; }

    public event Action<float, AuraCategory> OnAuraChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void AddAura(float amount)
    {
        currentAura = Mathf.Clamp(currentAura + amount, -100f, 100f);
        UpdateCategory();
    }

    private void UpdateCategory()
    {
        if (currentAura > 10f) Category = AuraCategory.Terang;
        else if (currentAura < -10f) Category = AuraCategory.Gelap;
        else Category = AuraCategory.Netral;

        OnAuraChanged?.Invoke(currentAura, Category);
    }
}