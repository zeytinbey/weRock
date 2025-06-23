using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyVaseDisplay : MonoBehaviour
{
    [Header("TOTAL VASE yazan UI Text")]
    public Text totalVaseText;

    void Start()
    {
        // 1) Se�ili slotu JSON'dan geri y�kle
        //    (SlotButton'la se�ti�iniz slot numaras� burada zaten currentSlot'ta)
        bool hasData = SaveManager.Instance.LoadGame(SaveManager.Instance.currentSlot);

        // 2) E�er kay�t varsa oradaki say�y� al, yoksa 0
        int count = hasData
            ? SaveManager.Instance.currentData.vaseCount
            : 0;

        // 3) UI'� g�ncelle
        totalVaseText.text = $"TOTAL VASE: {count}";
    }
}
