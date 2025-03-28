using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(!pausePanel.activeSelf);

            if (pausePanel.activeSelf)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
            }
        }
    }

    void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;                          // ?? Mouse görünür yap
        Cursor.lockState = CursorLockMode.None;         // ?? Ekran ortasýndan serbest býrak
        isPaused = true;
    }

    void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;                         // ?? Mouse tekrar gizle
        Cursor.lockState = CursorLockMode.Locked;       // ?? Tekrar ekran merkezine kilitle
        isPaused = false;
    }
}
