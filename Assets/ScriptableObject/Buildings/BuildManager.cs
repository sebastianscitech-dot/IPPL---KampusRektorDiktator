using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    public event Action<bool> OnBuildModeToggled;

    [Header("Data")]
    [SerializeField] public GameObject buildingPrefab;
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
        if (GameManager.Instance != null && GameManager.Instance.IsEventActive) return;

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            isBuildMode = !isBuildMode;
            OnBuildModeToggled?.Invoke(isBuildMode);
            Debug.Log($"Build Mode: {(isBuildMode ? "AKTIF" : "MATI")}");
        }

        if (isBuildMode && Mouse.current.leftButton.wasPressedThisFrame)
        {
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