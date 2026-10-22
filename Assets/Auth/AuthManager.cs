using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Untuk Input Field

public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputNama;

    public void Login()
    {
        // Ambil teks dari input field
        string namaRektor = inputNama.text;

        // Kalau kosong, kasih nama default
        if (string.IsNullOrEmpty(namaRektor))
        {
            namaRektor = "Rektor Abadi";
        }

        // Simpan nama ke PlayerPrefs (memori lokal Unity)
        PlayerPrefs.SetString("RektorName", namaRektor);
        PlayerPrefs.Save();

        Debug.Log($"Login Sukses sebagai: {namaRektor}");

        // Pindah ke Scene Gameplay
        SceneManager.LoadScene("Gameplay");
    }
}