using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class yaoshi : MonoBehaviour
{
    public enum KeyType { General, PipePuzzle }
    public KeyType keyType = KeyType.PipePuzzle;

    [Header("场景设置")]
    [Tooltip("切换到Beginning场景的延迟时间")]
    public float transitionDelay = 1f;

    private bool collected = false;

    void Awake()
    {
        // 初始化位置（根据截图中的管道位置调整）
        transform.localPosition = new Vector3(0.95f, -0.23f, 0f);
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        transform.localScale = new Vector3(0.00943f, 0.00943f, 0.00943f);

        // 确保有碰撞体
        if (GetComponent<Collider>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one * 0.3f;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("钥匙被点击"); // 验证点击事件
                    StartCoroutine(PickupKey());
                }
            }
        }
        // 增强点击检测
        if (Input.GetMouseButtonDown(0) && !collected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    StartCoroutine(PickupKey());
                }
            }
        }
    }

    IEnumerator PickupKey()
    {
        collected = true;

        // 缩放消失动画
        float timer = 0;
        Vector3 originalScale = transform.localScale;
        while (timer < transitionDelay)
        {
            timer += Time.deltaTime;
            transform.localScale = originalScale * (1 - timer / transitionDelay);
            yield return null;
        }

        // 保存状态并切换场景
        PlayerPrefs.SetInt("PipePuzzleCompleted", 1);
        SceneManager.LoadScene("Beginning");
        Destroy(gameObject);
    }

    // 调试用可视化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}