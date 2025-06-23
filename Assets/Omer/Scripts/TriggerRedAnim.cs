using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerRedAnim : MonoBehaviour
{
    [Header("Rengi deðiþecek objeler")]
    [Tooltip("Inspector’dan sürükle býrak ile atayacaðýn objeler")]
    public GameObject[] targets;

    [Header("Tetkiklendiðinde alacaklarý renk ve süre")]
    public Color triggeredColor = Color.red;
    public float duration = 2f;

    // Orijinal renkleri saklamak için
    private List<Color[]> originalColors = new List<Color[]>();
    private Collider col;
    private bool hasTriggered = false;

    void Awake()
    {
        // Collider'ý tetik olmasý için iþaretle
        col = GetComponent<Collider>();
        col.isTrigger = true;

        // Her hedef objenin Renderer’larýný bul, renklerini sakla
        foreach (var obj in targets)
        {
            var rends = obj.GetComponentsInChildren<Renderer>();
            Color[] cols = new Color[rends.Length];
            for (int i = 0; i < rends.Length; i++)
                cols[i] = rends[i].material.color;
            originalColors.Add(cols);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(RunColorChange());
        }
    }

    IEnumerator RunColorChange()
    {
        // 1) Tüm hedefleri tetik renk ile deðiþtir
        for (int i = 0; i < targets.Length; i++)
        {
            var rends = targets[i].GetComponentsInChildren<Renderer>();
            foreach (var r in rends)
                r.material.color = triggeredColor;
        }

        // 2) Süre kadar bekle
        yield return new WaitForSeconds(duration);

        // 3) Orijinal renklere geri döndür
        for (int i = 0; i < targets.Length; i++)
        {
            var rends = targets[i].GetComponentsInChildren<Renderer>();
            for (int j = 0; j < rends.Length; j++)
                rends[j].material.color = originalColors[i][j];
        }

        // 4) Bir daha tetiklenmesin diye Collider ve script’i kapat
        col.enabled = false;
        this.enabled = false;
    }
}