using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    [Header("Pause Panel (Canvas altýndaki)")]
    public GameObject panel;  // Inspector’dan buraya sürükle

    bool panelOpen = false;

    void Start()
    {
        if (panel == null)
        {
            Debug.LogError("UIControl: panel referansý atanmamýþ!");
            enabled = false;
            return;
        }

        panelOpen = false;
        panel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("UIControl Baþladý, panel kapalý.");
    }

    void Update()
    {
        // ? ESC her zaman buraya gelir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC algýlandý.");
            panelOpen = !panelOpen;
            panel.SetActive(panelOpen);

            if (panelOpen)
            {
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("Oyun durduruldu, panel açýk.");
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Debug.Log("Oyun devam ediyor, panel kapalý.");
            }
        }

        // Panel kapalýysa buradan çýk
        if (!panelOpen) return;

        // Panel açýksa 0 tuþunu dinle
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("0 algýlandý, ana menüye dönülüyor.");
            Time.timeScale = 1f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
        }
    }
}
