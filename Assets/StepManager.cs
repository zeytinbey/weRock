using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    [System.Serializable]
    public class StepGroup
    {
        public GameObject[] steps; // Her sýrada 3 basamak
    }

    public List<StepGroup> stepGroups = new List<StepGroup>();

    public Material normalMaterial;
    public Material highlightMaterial; // Kýrmýzý yanacak basamak

    private GameObject correctStep; // Doðru basamak

    void Start()
    {
        HighlightCorrectStep(0);
    }

    void HighlightCorrectStep(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= stepGroups.Count)
        {
            Debug.LogWarning("Geçersiz grup indeksi!");
            return;
        }

        StepGroup group = stepGroups[groupIndex];

        // Önce tüm basamaklarý normal materyale ayarla ve aktif yap
        foreach (var step in group.steps)
        {
            SetMaterial(step, normalMaterial);
            step.SetActive(true);
        }

        // Rastgele 1 doðru basamak seç
        int selectedIndex = Random.Range(0, group.steps.Length);
        correctStep = group.steps[selectedIndex];

        // Doðru basamaðý 1 saniye kýrmýzý yap sonra eski haline döndür
        StartCoroutine(FlashStep(correctStep, 1f));
    }

    IEnumerator FlashStep(GameObject step, float duration)
    {
        SetMaterial(step, highlightMaterial);
        yield return new WaitForSeconds(duration);
        SetMaterial(step, normalMaterial);
    }

    void SetMaterial(GameObject obj, Material mat)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = mat;
        }
    }

    public void OnStepPressed(GameObject step)
    {
        if (step == correctStep)
        {
            Debug.Log("Doðru basamaða bastýn!");
            // Doðru basamak için ekstra iþlem yapabilirsin
        }
        else
        {
            Debug.Log("Yanlýþ basamak! Basamak geçici olarak kapatýlýyor.");
            StartCoroutine(DisableStepTemporarily(step, 2f));
        }
    }

    IEnumerator DisableStepTemporarily(GameObject step, float duration)
    {
        Collider col = step.GetComponent<Collider>();
        if (col == null)
            col = step.GetComponentInChildren<Collider>();

        if (col != null)
            col.enabled = false;

        step.SetActive(false);

        yield return new WaitForSeconds(duration);

        step.SetActive(true);

        if (col != null)
            col.enabled = true;
    }
}
