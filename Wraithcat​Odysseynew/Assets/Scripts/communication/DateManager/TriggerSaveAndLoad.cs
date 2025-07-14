using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TriggerSaveAndLoad : MonoBehaviour
{
    public GameObject cat;
    public List<GameObject> objectsToSave;
    public string targetSceneName;

    private bool canTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == cat)
        {
            // 检查目标场景是否已经被访问过
            if (GameManager1.Instance.visitedScenes.ContainsKey(targetSceneName) &&
                !GameManager1.Instance.visitedScenes[targetSceneName])
            {
                // 保存游戏状态
                GameManager1.Instance.SaveGame(cat, objectsToSave);

                // 标记场景为已访问
                GameManager1.Instance.MarkSceneVisited(targetSceneName);

                // 加载目标场景
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }

    private void ResetTrigger()
    {
        canTrigger = true;
    }

    // 当物体被重新激活时重置触发状态
    private void OnEnable()
    {
        canTrigger = true;
    }
}