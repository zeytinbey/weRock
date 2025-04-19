using UnityEngine;
using UnityEngine.UI;

public class InteractUIController : MonoBehaviour
{
    public GameObject interactionPanel; // UI paneli (Canvas i�indeki)
    public float interactionDistance = 3f;
    private Transform player;

    private bool isPlayerInRange = false;
    public Button hatButton;
    public Button cigaretteButton;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        interactionPanel.SetActive(false);
    }

    void Update()
    {
        
        
        
        
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distance <= interactionDistance;




        // E�er "1" tu�una bas�ld�ysa
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Butonun click event'ini tetikle
            if (hatButton != null)
            {
                hatButton.onClick.Invoke();
                Debug.Log("1 tu�una bas�ld�, buton tetiklendi.");
            }
            else
            {
                Debug.LogWarning("targetButton atanmam��!");
            }
        }
        // E�er "2" tu�una bas�ld�ysa
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Butonun click event'ini tetikle
            if (cigaretteButton != null)
            {
                cigaretteButton.onClick.Invoke();
                Debug.Log("2 tu�una bas�ld�, buton tetiklendi.");
            }
            else
            {
                Debug.LogWarning("targetButton atanmam��!");
            }
        }


        if (interactionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape ile ClosePanel �a�r�l�yor");
            ClosePanel();
        }

       
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenPanel();
        }
    }

    void OpenPanel()
    {
        interactionPanel.SetActive(true);
        
        Time.timeScale = 0f; // Oyun devam etsin

    }

    public void ClosePanel()
    {
        Debug.Log("ClosePanel �a�r�ld�!");
        interactionPanel.SetActive(false);
        Time.timeScale = 1f; // Oyun devam etsin
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
