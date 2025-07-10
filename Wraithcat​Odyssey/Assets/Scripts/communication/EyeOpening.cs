using UnityEngine;

public class EyeOpening : MonoBehaviour
{
    // 左右两块黑幕
    public RectTransform leftMask;  // 左侧黑幕
    public RectTransform rightMask; // 右侧黑幕
    public float openSpeed = 500f;  // 打开速度

    private float targetX;    // 目标位置
    private bool isOpening = false; // 是否正在睁眼

    void Start()
    {

        
        targetX = Screen.width / 2f;  // 计算屏幕一半宽度
    }

    void Update()
    {
        if (!isOpening) return; // 没启动就不执行

        // 左右黑幕分别移动到目标位置
        leftMask.anchoredPosition = Vector2.MoveTowards(leftMask.anchoredPosition, new Vector2(-targetX, 0), openSpeed * Time.deltaTime);
        rightMask.anchoredPosition = Vector2.MoveTowards(rightMask.anchoredPosition, new Vector2(targetX, 0), openSpeed * Time.deltaTime);

        // 判断是否睁开完成
        if (Mathf.Abs(leftMask.anchoredPosition.x + targetX) < 1f)
        {
            leftMask.gameObject.SetActive(false);
            rightMask.gameObject.SetActive(false);
            isOpening = false;
        }
    }

    // 调用这个函数开始睁眼
    public void StartOpening()
    {
        isOpening = true;
    }
}