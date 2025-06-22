using UnityEngine;
using UnityEngine.UI;

public class ScreenCrackController : MonoBehaviour
{
    [Header("HP Bar")]
    [SerializeField] private Slider hpBar;

    [Header("Crack Effects (Raw Images)")]
    [Tooltip("E�ikleri s�ras�yla 100?80?60?40 i�in atay�n")]
    [SerializeField] private RawImage crackBelow100;
    [SerializeField] private RawImage crackBelow80;
    [SerializeField] private RawImage crackBelow60;
    [SerializeField] private RawImage crackBelow40;

    void Start()
    {
        if (hpBar == null) Debug.LogError("hpBar referans� atanmam��!", this);
        // �lk frame�de bir kez g�ncelle
        UpdateCracks(hpBar.value);

        // Slider�a de�er de�i�ince de g�ncelle
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

        // E�iklere g�re a�
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
        // else: hp == 100, hi�bir efekt yok
    }
}
