using System;

[Serializable]
public class StudentData
{
    public float posX;
    public float posY;
}

[Serializable]
public class BuildingSaveData
{
    public float posX;
    public float posY;
}

[Serializable]
public class SaveData
{
    public int money;
    public int humanCount;
    public float aura;
    public int currentDay;
    public string currentPhase;

    // TAMBAHAN: Array untuk nyimpen banyak mahasiswa & gedung
    public StudentData[] students;
    public BuildingSaveData[] buildings;
}