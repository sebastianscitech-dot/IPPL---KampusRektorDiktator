using UnityEngine;
using UnityEngine.EventSystems; // // Wajib untuk cek UI
using System;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    // TAMBAHAN: Event buat ngasih tau UI
    public event Action<bool> OnBuildModeToggled;

    [Header("Data")]
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private BuildingData testBuildingData;

    [Header("Build Mode")]
    public bool isBuildMode = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        // 1. Toggle Mode Bangun pakai tombol B
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildMode = !isBuildMode;
            Debug.Log($"Build Mode: {(isBuildMode ? "AKTIF" : "MATI")}");

            OnBuildModeToggled?.Invoke(isBuildMode);
        }

        // 2. Kalau mode bangun aktif, dan klik kiri
        if (isBuildMode && Input.GetMouseButtonDown(0))
        {
            // Cek apakah mouse sedang di atas UI? Kalau iya, jangan bangun.
            if (EventSystem.current.IsPointerOverGameObject()) return;

            PlaceBuilding();
        }
    }

    private void PlaceBuilding()
    {
        // Ambil posisi mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // SNAP TO GRID (Bikin kotaknya pas di tengah-tengah grid 1x1)
        mousePos.x = Mathf.Round(mousePos.x);
        mousePos.y = Mathf.Round(mousePos.y);

        // Cek Uang
        if (ResourceManager.Instance.SpendMoney(testBuildingData.cost))
        {
            GameObject newBuilding = Instantiate(buildingPrefab, mousePos, Quaternion.identity);
            newBuilding.name = testBuildingData.buildingName;
            Debug.Log($"Gedung {testBuildingData.buildingName} dibangun di {mousePos}");
        }
        else
        {
            Debug.LogWarning("Uang tidak cukup untuk membangun!");
        }
    }
}