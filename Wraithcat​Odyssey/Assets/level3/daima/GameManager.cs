using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("松子设置")]
    public int totalPineNuts = 5;
    [SerializeField] private int _collectedPineNuts = 0;

    [Header("钥匙设置")]
    public GameObject keyPrefab;
    private GameObject spawnedKey;
    public bool hasKey = false;
    public Vector3 keySpawnPosition = new Vector3(112f, 18f, 176f);

    [Header("UI 引用")]
    public Text pineNutText;
    public GameObject keyGetMask; // 新增：拖拽 KeyGetMask 面板到此处

    public bool shouldSpawnKey { get; private set; } = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ResetGameState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasKey = false;
        FindUIReferences();
        UpdatePineNutUI();

        if (shouldSpawnKey)
        {
            Debug.Log("场景加载：应在此场景生成钥匙");
            SpawnKeyInScene();
        }
    }

    void FindUIReferences()
    {
        // 动态查找 UI 元素（如果未手动分配）
        if (pineNutText == null)
        {
            GameObject uiTextObj = GameObject.FindGameObjectWithTag("PineNutCounter");
            if (uiTextObj != null) pineNutText = uiTextObj.GetComponent<Text>();
        }

        if (keyGetMask == null)
        {
            keyGetMask = GameObject.Find("KeyGetMask"); // 确保面板名称匹配
            if (keyGetMask != null) keyGetMask.SetActive(false); // 初始隐藏
        }
    }

    public void ResetGameState()
    {
        collectedPineNuts = 0;
        hasKey = false;
        shouldSpawnKey = false;

        if (keyGetMask != null) keyGetMask.SetActive(false);
        if (spawnedKey != null) Destroy(spawnedKey);
    }

    public int collectedPineNuts
    {
        get => _collectedPineNuts;
        private set
        {
            _collectedPineNuts = value;
            UpdatePineNutUI();
            CheckKeySpawnCondition();
        }
    }

    public void CollectPineNut(int value)
    {
        collectedPineNuts += value;
    }

    void UpdatePineNutUI()
    {
        if (pineNutText != null)
            pineNutText.text = $"松子: {collectedPineNuts}/{totalPineNuts}";
    }

    void CheckKeySpawnCondition()
    {
        if (!shouldSpawnKey && collectedPineNuts >= totalPineNuts && !hasKey)
        {
            shouldSpawnKey = true;
            SpawnKeyInScene();
        }
    }

    void SpawnKeyInScene()
    {
        if (!shouldSpawnKey || hasKey || keyPrefab == null) return;

        if (spawnedKey != null) Destroy(spawnedKey);

        spawnedKey = Instantiate(keyPrefab, keySpawnPosition, Quaternion.identity);
        shouldSpawnKey = false;
        Debug.Log($"钥匙生成于位置: {keySpawnPosition}");
    }

    public void ObtainKey()
    {
        hasKey = true;
        if (spawnedKey != null) Destroy(spawnedKey);

        if (keyGetMask != null)
        {
            keyGetMask.SetActive(true);
            Debug.Log("显示 KeyGetMask 面板");
        }
        else
        {
            Debug.LogWarning("KeyGetMask 面板未分配！");
        }
    }
}