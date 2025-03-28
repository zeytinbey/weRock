using UnityEngine;
using UnityEngine.UI; // UI Text kullan�m� i�in

public class CollectibleManager : MonoBehaviour
{
    // Sahneye g�re de�i�ebilecek maksimum toplanabilir obje say�s�
    [Header("Collectible Settings")]
    public int totalCollectibles = 10;

    // �u ana kadar toplanan obje say�s�
    private int collectedCount = 0;

    // UI �zerinde g�stermek i�in Text referans� (Inspector'da atanacak)
    [Header("UI")]
    public Text collectibleText;

    // Singleton gibi kullanmak isterseniz (opsiyonel):
    public static CollectibleManager Instance { get; private set; }

    private void Awake()
    {
        // Opsiyonel Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Ba�lang��ta UI g�ncellensin
        UpdateUI();
    }

    // Toplanabilir obje dokunuldu�unda bu metot �a�r�lacak
    public void CollectItem()
    {
        collectedCount++;
        if (collectedCount > totalCollectibles)
        {
            collectedCount = totalCollectibles;
        }
        UpdateUI();
    }

    // UI Text'i g�ncelleme
    private void UpdateUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = $"{collectedCount}/{totalCollectibles}";
        }
        else
        {
            Debug.LogWarning("CollectibleManager: UI Text referans� atanmam��!");
        }
    }
}
