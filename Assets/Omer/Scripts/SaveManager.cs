using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [HideInInspector] public int currentSlot = 1;      // Aktif slot numaras�
    [HideInInspector] public SaveData currentData = new SaveData();
    private const int FirstGameplaySceneIndex = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // E�er men� sahnesi ise (index < 2) kaydetme
        if (scene.buildIndex < FirstGameplaySceneIndex)
            return;

        // Aksi halde oyun sahnesi geliyor: kaydet
        currentData.sceneBuildIndex = scene.buildIndex;
        SaveGame(currentSlot);
    }

    // Kay�t dosya yolu: Application.persistentDataPath/save1.json, save2.json, save3.json
    private string GetPath(int slot) =>
        Path.Combine(Application.persistentDataPath, $"save{slot}.json");

    // currentData�y� disk�e yazar
    public void SaveGame(int slot)
    {
        var json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(GetPath(slot), json);
        Debug.Log($"[Kaydet] Slot {slot} ? Sahne {currentData.sceneBuildIndex}, Vazo {currentData.vaseCount}");
    }

    // Dosya varsa y�kler, yoksa false d�ner
    public bool LoadGame(int slot)
    {
        var path = GetPath(slot);
        if (!File.Exists(path)) return false;

        var json = File.ReadAllText(path);
        currentData = JsonUtility.FromJson<SaveData>(json);
        currentSlot = slot;
        Debug.Log($"[Y�kle] Slot {slot} ? Sahne {currentData.sceneBuildIndex}, Vazo {currentData.vaseCount}");
        return true;
    }

    // Kay�d� silmek isterseniz
    public void DeleteSave(int slot)
    {
        var path = GetPath(slot);
        if (File.Exists(path)) File.Delete(path);
    }
}
