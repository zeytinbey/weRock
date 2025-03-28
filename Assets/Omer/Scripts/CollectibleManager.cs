using UnityEngine;
using UnityEngine.UI; // UI Text kullanýmý için

public class CollectibleManager : MonoBehaviour
{
    // Sahneye göre deðiþebilecek maksimum toplanabilir obje sayýsý
    [Header("Collectible Settings")]
    public int totalCollectibles = 10;

    // Þu ana kadar toplanan obje sayýsý
    private int collectedCount = 0;

    // UI üzerinde göstermek için Text referansý (Inspector'da atanacak)
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
        // Baþlangýçta UI güncellensin
        UpdateUI();
    }

    // Toplanabilir obje dokunulduðunda bu metot çaðrýlacak
    public void CollectItem()
    {
        collectedCount++;
        if (collectedCount > totalCollectibles)
        {
            collectedCount = totalCollectibles;
        }
        UpdateUI();
    }

    // UI Text'i güncelleme
    private void UpdateUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = $"{collectedCount}/{totalCollectibles}";
        }
        else
        {
            Debug.LogWarning("CollectibleManager: UI Text referansý atanmamýþ!");
        }
    }
}
