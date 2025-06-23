using UnityEngine;
using UnityEngine.UI;

public class InteractUIController : MonoBehaviour
{
    [Header("Market Panel ve Butonlar")]
    public GameObject interactionPanel;
    public float interactionDistance = 3f;
    private Transform player;
    private bool isPlayerInRange = false;

    [Header("Item Butonlar�")]
    public Button hatButton;
    public Button cigaretteButton;
    public Button helmetButton;
    public Button pharaohButton;
    public Button britishButton;
    public Button horseButton;
    public Button kubaButton;
    public Button spearButton;

    [Header("Karakter ��indeki Objeler")]
    // Inspector'da s�rayla 1'den 8'e hangi GameObject ise ekle
    public GameObject[] itemObjects;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        interactionPanel.SetActive(false);

        // Panel a��ld���nda �nce hi�biri g�r�nmesin
        DeactivateAllItems();

        // (Opsiyonel) butonlara da listener ekleyebilirsin
        hatButton.onClick.AddListener(() => SelectItem(0));
        cigaretteButton.onClick.AddListener(() => SelectItem(1));
        helmetButton.onClick.AddListener(() => SelectItem(2));
        pharaohButton.onClick.AddListener(() => SelectItem(3));
        britishButton.onClick.AddListener(() => SelectItem(4));
        horseButton.onClick.AddListener(() => SelectItem(5));
        kubaButton.onClick.AddListener(() => SelectItem(6));
        spearButton.onClick.AddListener(() => SelectItem(7));
    }

    void Update()
    {
        // Panel a�ma / kapama
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distance <= interactionDistance;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            OpenPanel();
        if (interactionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            ClosePanel();

        // Klavyeden tu�la da se�me
        if (interactionPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectItem(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectItem(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectItem(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectItem(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) SelectItem(4);
            if (Input.GetKeyDown(KeyCode.Alpha6)) SelectItem(5);
            if (Input.GetKeyDown(KeyCode.Alpha7)) SelectItem(6);
            if (Input.GetKeyDown(KeyCode.Alpha8)) SelectItem(7);
        }
    }

    void OpenPanel()
    {
        interactionPanel.SetActive(true);
        Time.timeScale = 0f;
        // Panel a��ld���nda �nceki se�imi korumak istemezsen:
        // DeactivateAllItems();
    }

    public void ClosePanel()
    {
        interactionPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SelectItem(int index)
    {
        // �nce hepsini kapat
        DeactivateAllItems();
        // Sonra se�ileni a� (dizinin i�inde kontrol et)
        if (index >= 0 && index < itemObjects.Length)
            itemObjects[index].SetActive(true);
        Debug.Log($"{index + 1} tu�uyla item a��ld�.");
    }

    private void DeactivateAllItems()
    {
        foreach (var obj in itemObjects)
            obj.SetActive(false);
    }
}
