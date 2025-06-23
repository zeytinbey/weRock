using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;  // ? TMP namespace

public class SlotButton : MonoBehaviour
{
    [Tooltip("1, 2 veya 3")]
    public int slotIndex;

    [Tooltip("Buton altýndaki TMP Text")]
    public TextMeshProUGUI labelText;

    void Start()
    {
        RefreshLabel();
    }

    public void RefreshLabel()
    {
        // Eðer kayýt varsa veriyi yükle
        if (SaveManager.Instance.LoadGame(slotIndex))
        {
            var d = SaveManager.Instance.currentData;
            labelText.text = $"Level {d.sceneBuildIndex} – Vases {d.vaseCount}";
        }
        else
        {
            labelText.text = "Empty";
        }
    }

    public void OnClickSlot()
    {
        bool hasData = SaveManager.Instance.LoadGame(slotIndex);
        if (hasData)
        {
            // Kayýtlý sahneyi aç
            SceneManager.LoadScene(SaveManager.Instance.currentData.sceneBuildIndex);
        }
        else
        {
            // Yeni oyun baþlat
            SaveManager.Instance.currentSlot = slotIndex;
            SceneManager.LoadScene(1);
        }
    }

    public void OnClickDelete()
    {
        SaveManager.Instance.DeleteSave(slotIndex);
        RefreshLabel();
    }
}
