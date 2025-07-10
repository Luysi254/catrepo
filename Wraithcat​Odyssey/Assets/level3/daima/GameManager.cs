using System.Collections;
using System.Collections.Generic;
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

    // 新添加：控制钥匙生成状态的标志
    public bool shouldSpawnKey { get; private set; } = false;

    // 公共属性访问器
    public int collectedPineNuts
    {
        get => _collectedPineNuts;
        private set
        {
            _collectedPineNuts = value;
            UpdatePineNutUI();
            CheckKeySpawnCondition(); // 添加检查
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

    // 重置游戏状态
    public void ResetGameState()
    {
        collectedPineNuts = 0;
        hasKey = false;
        shouldSpawnKey = false;

        if (spawnedKey != null)
        {
            Destroy(spawnedKey);
            spawnedKey = null;
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasKey = false; // 每次加载场景重置钥匙持有状态

        // 查找UI引用
        FindUIReferences();
        UpdatePineNutUI();

        // 重要：检查是否需要在此场景生成钥匙
        if (shouldSpawnKey)
        {
            Debug.Log($"场景加载：应在此场景生成钥匙");
            SpawnKeyInScene();
        }
    }
    void FindUIReferences()
    {
        if (pineNutText != null) return; // 如果已找到，不再查找

        GameObject uiTextObj = GameObject.FindGameObjectWithTag("PineNutCounter");
        if (uiTextObj != null)
        {
            pineNutText = uiTextObj.GetComponent<Text>();
            Debug.Log("找到松子计数UI元素");
        }
        else
        {
            Debug.LogWarning("未找到松子计数UI元素");
        }
    }
    public void CollectPineNut(int value)
    {
        collectedPineNuts += value;
        Debug.Log($"收集松子: +{value} (总计: {collectedPineNuts}/{totalPineNuts})");
    }

    // 新方法：检查钥匙生成条件
    private void CheckKeySpawnCondition()
    {
        if (!shouldSpawnKey &&
            collectedPineNuts >= totalPineNuts &&
            !hasKey &&
            keyPrefab != null)
        {
            Debug.Log("已收集所有松子，触发钥匙生成");
            shouldSpawnKey = true;
            SpawnKeyInScene();
        }
    }
    void UpdatePineNutUI()
    {
        if (pineNutText != null)
            pineNutText.text = $"松子: {collectedPineNuts}/{totalPineNuts}";
    }

    // 修改后的钥匙生成方法
    void SpawnKeyInScene()
    {
        if (!shouldSpawnKey || hasKey)
        {
            Debug.LogWarning($"禁止生成钥匙: shouldSpawnKey={shouldSpawnKey}, hasKey={hasKey}");
            return;
        }

        if (spawnedKey != null)
        {
            Debug.Log("钥匙已存在，销毁旧实例");
            Destroy(spawnedKey);
            spawnedKey = null;
        }

        // 确保在正确位置生成
        Vector3 spawnPos = keySpawnPosition;
        Debug.Log($"生成钥匙于位置: {spawnPos}");
        spawnedKey = Instantiate(keyPrefab, spawnPos, Quaternion.identity);

        // 添加调试标记
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = spawnPos + Vector3.up * 2;
        marker.transform.localScale = Vector3.one * 0.5f;
        marker.GetComponent<Collider>().enabled = false;
        Destroy(marker, 10f);

        shouldSpawnKey = false; // 防止重复生成
    }
    public void ObtainKey()
    {
        hasKey = true;
        if (spawnedKey != null)
        {
            Destroy(spawnedKey);
            spawnedKey = null;
        }
    }


}