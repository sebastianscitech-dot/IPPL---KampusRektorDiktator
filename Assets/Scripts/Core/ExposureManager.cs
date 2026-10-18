using UnityEngine;
using System;

public class ExposureManager : MonoBehaviour
{
    public static ExposureManager Instance { get; private set; }

    [SerializeField] private float currentExposure = 0f;
    [SerializeField] private float ratePerInvestigator = 0.5f;

    public float Exposure => currentExposure;
    private int activeInvestigators = 0;

    public event Action<float> OnExposureChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (activeInvestigators > 0 && currentExposure < 100f)
        {
            currentExposure += ratePerInvestigator * activeInvestigators * Time.deltaTime;
            currentExposure = Mathf.Clamp(currentExposure, 0f, 100f);
            OnExposureChanged?.Invoke(currentExposure);
        }
    }

    public void RegisterInvestigator() => activeInvestigators++;
    public void UnregisterInvestigator() => activeInvestigators = Mathf.Max(0, activeInvestigators - 1);
    public void ReduceExposure(float amount)
    {
        currentExposure = Mathf.Clamp(currentExposure - amount, 0f, 100f);
        OnExposureChanged?.Invoke(currentExposure);
    }
}