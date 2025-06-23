using UnityEngine;
using UnityEngine.UI;

public class VaseCollectible : MonoBehaviour
{
    public Text vaseCountText;

    void Start()
    {
        UpdateVaseUI();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: {other.name} ile �arp��t�"); // ? eklendi

        if (!other.CompareTag("Player"))
            return;

        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance null! Scene�de SaveManager var m�?");
            return;
        }

        // Saya� ++
        SaveManager.Instance.currentData.vaseCount++;
        Debug.Log($"Yeni vazo say�s�: {SaveManager.Instance.currentData.vaseCount}"); // ? eklendi

        // Kaydet
        SaveManager.Instance.SaveGame(SaveManager.Instance.currentSlot);

        // UI�y� g�ncelle
        UpdateVaseUI();

        // Obje yok et
        Destroy(gameObject);
    }

    void UpdateVaseUI()
    {
        if (vaseCountText == null)
        {
            Debug.LogWarning("vaseCountText atanmam��!");
            return;
        }

        int total = SaveManager.Instance.currentData.vaseCount;
        vaseCountText.text = "TOTAL VASE: " + total;
        // Alternatif interpolasyon:
        // vaseCountText.text = $"TOTAL VASE: {total}";
    }
}
