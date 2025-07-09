using UnityEngine;

public class KeyFloatAndRotate : MonoBehaviour
{
    // 漂浮参数
    public float floatHeight = 0.5f;    // 上下浮动高度
    public float floatSpeed = 1f;       // 浮动速度
    private Vector3 startPos;          // 初始位置

    public GameObject keyGetMask; // 钥匙遮罩提示
    public float maskDelay = 1.0f;    // 遮罩显示延迟时间（秒）

    // 旋转参数
    public float rotateSpeed = 90f;    // 旋转速度（度/秒）

    void Start()
    {
        startPos = transform.position; // 记录初始位置
        if (keyGetMask != null)
        {
            keyGetMask.SetActive(false); // 确保遮罩初始隐藏
        }
    }

    void Update()
    {
        // 上下漂浮效果（使用正弦函数）
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // 绕世界Y轴旋转（忽略物体自身旋转）
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    // 触碰钥匙
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保小猫Tag是"Player"
        {
            // 隐藏钥匙
            gameObject.SetActive(false);

            // 延迟显示遮罩提示
            if (keyGetMask != null)
            {
                Invoke("ShowMask", maskDelay);
            }
        }
    }

    void ShowMask()
    {
        keyGetMask.SetActive(true);
    }
}