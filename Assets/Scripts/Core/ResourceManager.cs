using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public void SetData(int savedMoney, int savedHuman)
    {
        Money = savedMoney;
        HumanCount = savedHuman;
        OnResourceChanged?.Invoke();
    }
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

    public void RemoveHuman(int amount)
    {
        HumanCount = Mathf.Max(0, HumanCount - amount);
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

    public void ForceSpendMoney(int amount)
    {
        Money -= amount;
        OnResourceChanged?.Invoke();
    }

    public void AddHuman(int amount)
    {
        HumanCount += amount;
        OnResourceChanged?.Invoke();
    }
}