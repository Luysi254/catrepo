using UnityEngine;
using UnityEngine.UI; // 注意使用UnityEngine.UI命名空间

public class PulsingButton : MonoBehaviour
{
    public float pulseSpeed = 1f;
    public float minScale = 0.95f;
    public float maxScale = 1.05f;

    private RectTransform rectTransform;
    private Vector3 originalScale;

    void Start()
    {
        // 获取按钮的RectTransform
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    void Update()
    {
        // 使用正弦函数实现平滑脉冲
        float scale = Mathf.Lerp(minScale, maxScale,
            (Mathf.Sin(Time.time * pulseSpeed) + 1) / 2);

        rectTransform.localScale = originalScale * scale;
    }
}