using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;

    // 玩家数据
    public Vector3 catPosition;
    public Quaternion catRotation;

    // 场景状态
    public Dictionary<string, bool> objectStates = new Dictionary<string, bool>();
    public Dictionary<string, bool> visitedScenes = new Dictionary<string, bool>();
    public bool showEndUI = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 初始化场景访问状态
        visitedScenes = new Dictionary<string, bool>()
          {
            {"C1", false},
            {"Level1", false},
            {"Level2", false},
            {"Level3", false}
           };
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    public void SaveGame(GameObject cat, List<GameObject> objectsToSave)
    {
        catPosition = cat.transform.position;
        catRotation = cat.transform.rotation;

        objectStates.Clear();
        foreach (var obj in objectsToSave)
        {
            objectStates[obj.name] = obj.activeSelf;
        }
    }

    public void LoadGame(GameObject cat, List<GameObject> objectsToLoad)
    {
        if (cat != null)
        {
            cat.transform.position = catPosition;
            cat.transform.rotation = catRotation;
        }

        foreach (var obj in objectsToLoad)
        {
            if (objectStates.ContainsKey(obj.name))
                obj.SetActive(objectStates[obj.name]);
        }
    }

    public void MarkSceneVisited(string sceneName)
    {
        if (visitedScenes.ContainsKey(sceneName))
        {
            visitedScenes[sceneName] = true;
        }
    }
}