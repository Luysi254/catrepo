using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;       // 移动速度
    public float moveUpSpeed = 3f;     // 向上移动速度
    public float floatHeight = 0.5f;   // 浮动高度（上下总范围）
    public float floatSpeed = 1f;      // 浮动速度（频率）

    private float originalY;           // 浮动基准位置
    private float floatTimer = 0f;     // 浮动计时器
    private bool isFloating = false;   // 是否正在浮动

   
    void Start()
    {
        originalY = transform.position.y; // 初始化基准位置
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D 或 左右键
        float moveY = Input.GetAxisRaw("Vertical");   // W/S 或 上下键

        // 如果有输入，停止浮动并移动
        if (moveX != 0f || moveY != 0f)
        {
            isFloating = false;
            floatTimer = 0f; // 重置计时器
            Vector3 move = new Vector3(moveX * moveSpeed, moveY * moveUpSpeed, 0f);
            transform.position += move * Time.deltaTime;
        }
        else
        {
            // 无输入时，开始浮动
            if (!isFloating)
            {
                originalY = transform.position.y; // 更新基准位置
                isFloating = true;
            }
            FloatEffect();
        }

        // 限制Y轴范围（-316到-148）
        float clampedY = Mathf.Clamp(transform.position.y, -316f, -148f);
        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }

    void FloatEffect()
    {
        // 使用正弦波 + 缓动，让浮动更柔和
        floatTimer += Time.deltaTime * floatSpeed;
        float newY = originalY + Mathf.Sin(floatTimer) * floatHeight * 0.5f; // 高度减半，避免突变
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}