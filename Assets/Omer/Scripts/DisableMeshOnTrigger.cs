using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerThenDisappear : MonoBehaviour
{
    [Header("Etkilenecek Objeler")]
    public GameObject[] targets;

    [Header("Kaç Objeyi Kapatmak Ýstersiniz? (0 = tümü)")]
    public int disableCount = 0;

    [Header("Kaç Saniye Sonra Kaybolsun?")]
    public float delay = 2f;

    [Header("Trigger Anýnda Alacaðý Renk")]
    public Color triggerColor = Color.red;

    private bool hasTriggered = false;
    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        // 1) Anýnda rengi deðiþtir
        ApplyColor(triggerColor);

        // 2) Sonra kaybolma coroutine'ini baþlat
        StartCoroutine(DisableAfterDelay());
    }

    void ApplyColor(Color color)
    {
        // Kaç objenin etkileneceðini hesapla
        int count = (disableCount <= 0)
            ? targets.Length
            : Mathf.Clamp(disableCount, 0, targets.Length);

        for (int i = 0; i < count; i++)
        {
            foreach (var r in targets[i].GetComponentsInChildren<Renderer>())
            {
                var mat = r.material;
                // URP Lit shader için:
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", color);
                // Standard shader için:
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", color);
            }
        }
    }

    IEnumerator DisableAfterDelay()
    {
        // delay kadar bekle
        yield return new WaitForSeconds(delay);

        // MeshRenderer'larý kapat
        int count = (disableCount <= 0)
            ? targets.Length
            : Mathf.Clamp(disableCount, 0, targets.Length);

        for (int i = 0; i < count; i++)
            foreach (var r in targets[i].GetComponentsInChildren<Renderer>())
                r.enabled = false;

        // Bir daha tetiklenmesin
        col.enabled = false;
        enabled = false;
    }
}
