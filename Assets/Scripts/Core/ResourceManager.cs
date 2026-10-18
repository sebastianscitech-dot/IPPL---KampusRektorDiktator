using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private int startingMoney = 100;
    [SerializeField] private int startingHuman = 0;

    public int Money { get; private set; }
    public int HumanCount { get; private set; }

    public event Action OnResourceChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        Money = startingMoney;
        HumanCount = startingHuman;
        OnResourceChanged?.Invoke();
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        OnResourceChanged?.Invoke();
    }

    public bool SpendMoney(int amount)
    {
        if (Money >= amount && amount >= 0) // Biaya gak minus
        {
            Money -= amount;
            if (Money < 0) Money = 0;
            OnResourceChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void AddHuman(int amount)
    {
        HumanCount += amount;
        OnResourceChanged?.Invoke();
    }
}