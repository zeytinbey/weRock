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
        Debug.Log($"OnTriggerEnter: {other.name} ile çarpýþtý"); // ? eklendi

        if (!other.CompareTag("Player"))
            return;

        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance null! Scene’de SaveManager var mý?");
            return;
        }

        // Sayaç ++
        SaveManager.Instance.currentData.vaseCount++;
        Debug.Log($"Yeni vazo sayýsý: {SaveManager.Instance.currentData.vaseCount}"); // ? eklendi

        // Kaydet
        SaveManager.Instance.SaveGame(SaveManager.Instance.currentSlot);

        // UI’yý güncelle
        UpdateVaseUI();

        // Obje yok et
        Destroy(gameObject);
    }

    void UpdateVaseUI()
    {
        if (vaseCountText == null)
        {
            Debug.LogWarning("vaseCountText atanmamýþ!");
            return;
        }

        int total = SaveManager.Instance.currentData.vaseCount;
        vaseCountText.text = "TOTAL VASE: " + total;
        // Alternatif interpolasyon:
        // vaseCountText.text = $"TOTAL VASE: {total}";
    }
}
