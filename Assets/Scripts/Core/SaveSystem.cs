using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        savePath = Application.persistentDataPath + "/savegame.json";
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        if (ResourceManager.Instance != null)
        {
            data.money = ResourceManager.Instance.Money;
            data.humanCount = ResourceManager.Instance.HumanCount;
        }
        if (AuraManager.Instance != null) data.aura = AuraManager.Instance.Aura;
        if (GameManager.Instance != null)
        {
            data.currentDay = GameManager.Instance.CurrentDay;
            data.currentPhase = GameManager.Instance.CurrentPhase.ToString();
        }

        // --- SIMPAN MAHASISWA ---
        StudentAI[] studentsInScene = FindObjectsByType<StudentAI>(FindObjectsInactive.Exclude);
        data.students = new StudentData[studentsInScene.Length];
        for (int i = 0; i < studentsInScene.Length; i++)
        {
            data.students[i] = new StudentData
            {
                posX = studentsInScene[i].transform.position.x,
                posY = studentsInScene[i].transform.position.y
            };
        }

        // --- SIMPAN GEDUNG ---
        Building[] buildingsInScene = FindObjectsByType<Building>(FindObjectsInactive.Exclude);
        data.buildings = new BuildingSaveData[buildingsInScene.Length];
        for (int i = 0; i < buildingsInScene.Length; i++)
        {
            data.buildings[i] = new BuildingSaveData
            {
                posX = buildingsInScene[i].transform.position.x,
                posY = buildingsInScene[i].transform.position.y
            };
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game Disimpan! ({data.students.Length} Mahasiswa, {data.buildings.Length} Gedung)");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 1. Kembalikan Angka
        if (ResourceManager.Instance != null) ResourceManager.Instance.SetData(data.money, data.humanCount);
        if (AuraManager.Instance != null) AuraManager.Instance.SetData(data.aura);
        if (GameManager.Instance != null) GameManager.Instance.SetData(data.currentDay, data.currentPhase);

        // 2. HANCURKAN OBJEK LAMA DI SCENE
        StudentAI[] oldStudents = FindObjectsByType<StudentAI>(FindObjectsInactive.Exclude);
        foreach (var s in oldStudents) Destroy(s.gameObject);

        Building[] oldBuildings = FindObjectsByType<Building>(FindObjectsInactive.Exclude);
        foreach (var b in oldBuildings) Destroy(b.gameObject);

        // 3. MUNCULKAN KEMBALI DARI DATA
        if (GameManager.Instance != null && GameManager.Instance.studentPrefab != null)
        {
            foreach (var sData in data.students)
            {
                Instantiate(GameManager.Instance.studentPrefab, new Vector3(sData.posX, sData.posY, 0), Quaternion.identity);
            }
        }

        if (BuildManager.Instance != null && BuildManager.Instance.buildingPrefab != null)
        {
            foreach (var bData in data.buildings)
            {
                GameObject newBuilding = Instantiate(BuildManager.Instance.buildingPrefab, new Vector3(bData.posX, bData.posY, 0), Quaternion.identity);
            }
        }

        Debug.Log("Game Berhasil Dimuat beserta Gedung & Mahasiswanya!");
    }
}