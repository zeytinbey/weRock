using UnityEngine;
using UnityEngine.UI;

public class ScreenCrackController : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Slider hpBar;

    [Header("Crack Effects (Raw Images)")]
    [Tooltip("Eþikleri sýrasýyla 100?80?60?40 için atayýn")]
    [SerializeField] private RawImage crackBelow100;
    [SerializeField] private RawImage crackBelow80;
    [SerializeField] private RawImage crackBelow60;
    [SerializeField] private RawImage crackBelow40;

    void Start()
    {
        if (hpBar == null) Debug.LogError("hpBar referansý atanmamýþ!", this);
        // Ýlk frame’de bir kez güncelle
        UpdateCracks(hpBar.value);

        // Slider’a deðer deðiþince de güncelle
        hpBar.onValueChanged.AddListener(UpdateCracks);
    }

    void OnDestroy()
    {
        if (hpBar != null)
            hpBar.onValueChanged.RemoveListener(UpdateCracks);
    }

    private void UpdateCracks(float currentHp)
    {
        // Hepsini kapat
        crackBelow100.gameObject.SetActive(false);
        crackBelow80.gameObject.SetActive(false);
        crackBelow60.gameObject.SetActive(false);
        crackBelow40.gameObject.SetActive(false);

        // Eþiklere göre aç
        if (currentHp < 10f)
        {
            crackBelow40.gameObject.SetActive(true);
        }
        else if (currentHp < 20f)
        {
            crackBelow60.gameObject.SetActive(true);
        }
        else if (currentHp < 30f)
        {
            crackBelow80.gameObject.SetActive(true);
        }
        else if (currentHp < 50f)
        {
            crackBelow100.gameObject.SetActive(true);
        }
        // else: hp == 100, hiçbir efekt yok
    }
}
