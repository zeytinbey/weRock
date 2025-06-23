using UnityEngine;
using UnityEngine.UI;

public class InteractUIController : MonoBehaviour
{
    [Header("Market Panel ve Butonlar")]
    public GameObject interactionPanel;
    public float interactionDistance = 3f;
    private Transform player;
    private bool isPlayerInRange = false;

    [Header("Item Butonlarý")]
    public Button hatButton;
    public Button cigaretteButton;
    public Button helmetButton;
    public Button pharaohButton;
    public Button britishButton;
    public Button horseButton;
    public Button kubaButton;
    public Button spearButton;

    [Header("Karakter Ýçindeki Objeler")]
    // Inspector'da sýrayla 1'den 8'e hangi GameObject ise ekle
    public GameObject[] itemObjects;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        interactionPanel.SetActive(false);

        // Panel açýldýðýnda önce hiçbiri görünmesin
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
        // Panel açma / kapama
        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerInRange = distance <= interactionDistance;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            OpenPanel();
        if (interactionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            ClosePanel();

        // Klavyeden tuþla da seçme
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
        // Panel açýldýðýnda önceki seçimi korumak istemezsen:
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
        // Önce hepsini kapat
        DeactivateAllItems();
        // Sonra seçileni aç (dizinin içinde kontrol et)
        if (index >= 0 && index < itemObjects.Length)
            itemObjects[index].SetActive(true);
        Debug.Log($"{index + 1} tuþuyla item açýldý.");
    }

    private void DeactivateAllItems()
    {
        foreach (var obj in itemObjects)
            obj.SetActive(false);
    }
}
