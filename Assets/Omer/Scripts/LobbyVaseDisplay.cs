using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyVaseDisplay : MonoBehaviour
{
    [Header("TOTAL VASE yazan UI Text")]
    public Text totalVaseText;

    void Start()
    {
        // 1) Seçili slotu JSON'dan geri yükle
        //    (SlotButton'la seçtiðiniz slot numarasý burada zaten currentSlot'ta)
        bool hasData = SaveManager.Instance.LoadGame(SaveManager.Instance.currentSlot);

        // 2) Eðer kayýt varsa oradaki sayýyý al, yoksa 0
        int count = hasData
            ? SaveManager.Instance.currentData.vaseCount
            : 0;

        // 3) UI'ý güncelle
        totalVaseText.text = $"TOTAL VASE: {count}";
    }
}
