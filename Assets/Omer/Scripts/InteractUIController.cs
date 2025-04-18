using UnityEngine;

public class InteractUIController : MonoBehaviour
{
    public GameObject interactionPanel; // UI paneli (Canvas içindeki)
    public float interactionDistance = 3f;
    private Transform player;

    private bool isPlayerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        interactionPanel.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distance <= interactionDistance;

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
        Cursor.lockState = CursorLockMode.None; // Fare serbest kalsýn
        Cursor.visible = true;
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
