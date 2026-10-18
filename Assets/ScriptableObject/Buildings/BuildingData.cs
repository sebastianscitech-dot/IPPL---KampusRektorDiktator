using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "ITK/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public int cost;
    public Vector2Int size; // Misal 1x1, 2x2
    public int auraPerDay;
}