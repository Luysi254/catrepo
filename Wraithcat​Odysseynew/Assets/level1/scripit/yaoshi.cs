using UnityEngine;
using System.Collections;

public class yaoshi : MonoBehaviour
{
    public enum KeyType { General, PipePuzzle }
    public KeyType keyType = KeyType.PipePuzzle;

    [Header("UI设置")]
    [Tooltip("切换到KeyGetMask画板的延迟时间")]
    public float transitionDelay = 1f;

    private GameObject keyGetMaskCanvas;
    private bool collected = false;

    void Awake()
    {
        // 保持预制体初始位置（根据截图中的数值）
        transform.localPosition = new Vector3(0.95f, -0.23f, 0f);
        transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        transform.localScale = new Vector3(0.00943f, 0.00943f, 0.00943f);

        // 确保物理组件存在（匹配截图中的Rigidbody和Collider）
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // 防止物理影响
        }

        if (GetComponent<Collider>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one * 0.3f;
        }
    }

    void Start()
    {
        // 自动查找画板（匹配截图中的"KeyGetMask"对象）
        keyGetMaskCanvas = GameObject.Find("KeyGetMask");

        // 备用查找方案（如果画板在Canvas子物体中）
        if (keyGetMaskCanvas == null)
        {
            Transform canvas = GameObject.Find("Canvas")?.transform;
            if (canvas != null) keyGetMaskCanvas = canvas.Find("KeyGetMask")?.gameObject;
        }

        if (keyGetMaskCanvas != null)
        {
            keyGetMaskCanvas.SetActive(false); // 初始隐藏
        }
        else
        {
            Debug.LogError("自动查找失败！请确认：1.画板命名为'KeyGetMask' 2.画板存在于场景中");
        }
    }

    void OnMouseDown()
    {
        if (!collected) StartCoroutine(PickupKey());
    }

    IEnumerator PickupKey()
    {
        collected = true;

        // 缩放消失动画
        Vector3 originalScale = transform.localScale;
        float timer = 0;
        while (timer < transitionDelay)
        {
            timer += Time.deltaTime;
            transform.localScale = originalScale * (1 - timer / transitionDelay);
            yield return null;
        }

        // 安全激活画板
        if (keyGetMaskCanvas != null)
        {
            keyGetMaskCanvas.SetActive(true);
            PlayerPrefs.SetInt("PipePuzzleCompleted", 1);
        }
        else
        {
            Debug.LogWarning("画板激活失败，但继续执行销毁");
        }

        Destroy(gameObject);
    }

    // 调试可视化（可选）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}