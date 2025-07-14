using UnityEngine;

public class FloatingButton : MonoBehaviour
{
    public float floatSpeed = 1f;      // 浮动速度
    public float floatAmount = 10f;   // 浮动幅度

    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // 使用正弦函数创建平滑的上下浮动效果
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        rectTransform.anchoredPosition = startPosition + new Vector2(0, yOffset);
    }
}