using UnityEngine;
using UnityEngine.UI;

public class InteractUIController : MonoBehaviour
{
    public GameObject interactionPanel; // UI paneli (Canvas içindeki)
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




        // Eðer "1" tuþuna basýldýysa
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Butonun click event'ini tetikle
            if (hatButton != null)
            {
                hatButton.onClick.Invoke();
                Debug.Log("1 tuþuna basýldý, buton tetiklendi.");
            }
            else
            {
                Debug.LogWarning("targetButton atanmamýþ!");
            }
        }
        // Eðer "2" tuþuna basýldýysa
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Butonun click event'ini tetikle
            if (cigaretteButton != null)
            {
                cigaretteButton.onClick.Invoke();
                Debug.Log("2 tuþuna basýldý, buton tetiklendi.");
            }
            else
            {
                Debug.LogWarning("targetButton atanmamýþ!");
            }
        }


        if (interactionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape ile ClosePanel çaðrýlýyor");
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
        Debug.Log("ClosePanel çaðrýldý!");
        interactionPanel.SetActive(false);
        Time.timeScale = 1f; // Oyun devam etsin
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
