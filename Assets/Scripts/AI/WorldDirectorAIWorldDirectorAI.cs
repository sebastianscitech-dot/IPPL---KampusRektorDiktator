using UnityEngine;
using System.Collections.Generic;

public class WorldDirectorAI : MonoBehaviour
{
    public static WorldDirectorAI Instance { get; private set; }

    [Header("Event Pool")]
    [SerializeField] private List<GameEvent> allEvents = new List<GameEvent>();

    [Header("Investigator Settings")]
    public GameObject investigatorPrefab;
    public bool spawnInvestigatorNextEvent = false;

    public int investigatorSpawnBonus = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public GameEvent PickRandomEvent()
    {
        if (allEvents.Count == 0) return null;

        float aura = AuraManager.Instance.Aura;
        // Rumus dari SRS: t = (aura + 100) / 200 (0 = Gelap, 1 = Terang)
        float t = (aura + 100f) / 200f;

        float totalWeight = 0f;
        List<float> weights = new List<float>();

        // Hitung bobot tiap event
        foreach (var evt in allEvents)
        {
            float weight = evt.baseWeight;
            if (evt.category == GameEvent.EventCategory.Positif)
                weight *= Mathf.Lerp(0.3f, 2f, t);
            else if (evt.category == GameEvent.EventCategory.Negatif)
                weight *= Mathf.Lerp(2f, 0.3f, t);
            // Event "Aneh" bobotnya tetap

            weights.Add(weight);
            totalWeight += weight;
        }

        // Pilih pakai Roulette Wheel
        float randomPoint = Random.Range(0f, totalWeight);
        for (int i = 0; i < allEvents.Count; i++)
        {
            if (randomPoint < weights[i])
                return allEvents[i];
            randomPoint -= weights[i];
        }


        return allEvents[0]; // Fallback
    }
    public void SpawnInvestigator()
    {
        if (investigatorPrefab != null)
        {
            int totalToSpawn = 1 + investigatorSpawnBonus; // Total spawn = 1 + bonus penalti

            for (int i = 0; i < totalToSpawn; i++)
            {
                Vector3 spawnPos = new Vector3(15f, 10f + (i * 2f), 0f); // Kasih jarak dikit biar gak tumpuk
                Instantiate(investigatorPrefab, spawnPos, Quaternion.identity);
            }

            Debug.LogWarning($"INVESTIGATOR DATANG! Total: {totalToSpawn} (Bonus: {investigatorSpawnBonus})");
        }
    }
}