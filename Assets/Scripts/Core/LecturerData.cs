using UnityEngine;

[System.Serializable]
public class LecturerData
{
    public string lecturerName;
    public float compliance;
    public float auraPerDay;
    public int salary;
    public bool canWorkNight; // TAMBAHAN: Bisa kerja malam atau nggak

    public static LecturerData GenerateRandomLecturer(int day, bool isNightOwl)
    {
        LecturerData newDosen = new LecturerData();

        string[] names = { "Dr. Strange", "Prof. Killer", "Bu Dosen Galak", "Pak Santai", "Ir. Robot", "Dosen Abadi", "Prof. Tidur", "Dr. Kalkulus", "Pak Fajri..." };
        newDosen.lecturerName = names[Random.Range(0, names.Length)];

        // Hari 1 skill rendah, makin tinggi hari makin gila skillnya
        newDosen.compliance = Random.Range(50f, 100f);
        newDosen.auraPerDay = Random.Range(1f, 3f) + (day * 0.2f);

        // Gaji proporsional sama kualitas (1 Aura = 15 Uang)
        newDosen.salary = Mathf.RoundToInt(newDosen.auraPerDay * 15f);

        newDosen.canWorkNight = isNightOwl;

        return newDosen;
    }
}