using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerThenDisappear : MonoBehaviour
{
    [Header("Etkilenecek Objeler")]
    public GameObject[] targets;

    [Header("Ka� Objeyi Kapatmak �stersiniz? (0 = t�m�)")]
    public int disableCount = 0;

    [Header("Ka� Saniye Sonra Kaybolsun?")]
    public float delay = 2f;

    [Header("Trigger An�nda Alaca�� Renk")]
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

        // 1) An�nda rengi de�i�tir
        ApplyColor(triggerColor);

        // 2) Sonra kaybolma coroutine'ini ba�lat
        StartCoroutine(DisableAfterDelay());
    }

    void ApplyColor(Color color)
    {
        // Ka� objenin etkilenece�ini hesapla
        int count = (disableCount <= 0)
            ? targets.Length
            : Mathf.Clamp(disableCount, 0, targets.Length);

        for (int i = 0; i < count; i++)
        {
            foreach (var r in targets[i].GetComponentsInChildren<Renderer>())
            {
                var mat = r.material;
                // URP Lit shader i�in:
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", color);
                // Standard shader i�in:
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", color);
            }
        }
    }

    IEnumerator DisableAfterDelay()
    {
        // delay kadar bekle
        yield return new WaitForSeconds(delay);

        // MeshRenderer'lar� kapat
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
