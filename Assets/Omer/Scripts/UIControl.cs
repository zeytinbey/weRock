using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    [Header("Pause Panel (Canvas alt�ndaki)")]
    public GameObject panel;  // Inspector�dan buraya s�r�kle

    bool panelOpen = false;

    void Start()
    {
        if (panel == null)
        {
            Debug.LogError("UIControl: panel referans� atanmam��!");
            enabled = false;
            return;
        }

        panelOpen = false;
        panel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("UIControl Ba�lad�, panel kapal�.");
    }

    void Update()
    {
        // ? ESC her zaman buraya gelir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC alg�land�.");
            panelOpen = !panelOpen;
            panel.SetActive(panelOpen);

            if (panelOpen)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("Oyun durduruldu, panel a��k.");
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Debug.Log("Oyun devam ediyor, panel kapal�.");
            }
        }

        // Panel kapal�ysa buradan ��k
        if (!panelOpen) return;

        // Panel a��ksa 0 tu�unu dinle
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("0 alg�land�, ana men�ye d�n�l�yor.");
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
        }
    }
}
