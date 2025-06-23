using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    [System.Serializable]
    public class StepGroup
    {
        public GameObject[] steps; // Her s�rada 3 basamak
    }

    public List<StepGroup> stepGroups = new List<StepGroup>();

    public Material normalMaterial;
    public Material highlightMaterial; // K�rm�z� yanacak basamak

    private GameObject correctStep; // Do�ru basamak

    void Start()
    {
        HighlightCorrectStep(0);
    }

    void HighlightCorrectStep(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= stepGroups.Count)
        {
            Debug.LogWarning("Ge�ersiz grup indeksi!");
            return;
        }

        StepGroup group = stepGroups[groupIndex];

        // �nce t�m basamaklar� normal materyale ayarla ve aktif yap
        foreach (var step in group.steps)
        {
            SetMaterial(step, normalMaterial);
            step.SetActive(true);
        }

        // Rastgele 1 do�ru basamak se�
        int selectedIndex = Random.Range(0, group.steps.Length);
        correctStep = group.steps[selectedIndex];

        // Do�ru basama�� 1 saniye k�rm�z� yap sonra eski haline d�nd�r
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
            Debug.Log("Do�ru basama�a bast�n!");
            // Do�ru basamak i�in ekstra i�lem yapabilirsin
        }
        else
        {
            Debug.Log("Yanl�� basamak! Basamak ge�ici olarak kapat�l�yor.");
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
