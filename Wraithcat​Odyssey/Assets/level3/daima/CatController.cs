using UnityEngine;

public class CatController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f; // 移动速度
    public float rotationSpeed = 10f; // 旋转速度

    [Header("浮动动画设置")]
    public float floatHeight = 0.2f;      // 浮动高度（米）
    public float floatSpeed = 1.5f;       // 浮动速度（每秒周期数）
    public float floatSmoothness = 5f;    // 浮动平滑度
    public bool enableFloating = true;    // 是否启用浮动动画

    private Rigidbody rb; // 刚体组件
    private Quaternion baseRotation; // 存储基础旋转偏移
    private Vector3 originalPosition;     // 原始位置（用于浮动计算）
    private float floatOffset;            // 浮动偏移量（用于正弦计算）
    private bool isFloating = false;      // 当前是否正在浮动

    [Header("陷阱机制")]
    public bool isTrapped = false;              // 是否被禁锢
    public float trapDuration = 3f;              // 禁锢持续时间
    private float trapTimer;                      // 禁锢计时器
    public int escapePresses = 5;                 // 逃脱需要连按的次数
    private int currentPressCount;                // 当前按键计数

    void Start()
    {
        // 设置基础旋转偏移（绕X轴旋转-90度）
        baseRotation = Quaternion.Euler(-90, 0, 0);

        // 应用初始旋转
        transform.rotation = baseRotation;

        // 获取刚体组件
        rb = GetComponent<Rigidbody>();

        // 冻结旋转以防止物理影响旋转
        rb.freezeRotation = true;

        // 记录初始位置作为浮动基准
        originalPosition = transform.position;
    }

    void Update()
    {
        // 获取WASD输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 计算移动方向
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        // 检查是否应该浮动（没有移动输入且未被禁锢）
        isFloating = enableFloating && moveDirection == Vector3.zero && !isTrapped;

        // 移动角色
        if (moveDirection != Vector3.zero)
        {
            // 移动位置
            Vector3 moveVelocity = moveDirection * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

            // 直接计算目标旋转（保留X轴-90度，仅更新Y轴）
            Quaternion targetRotation = Quaternion.Euler(-90,
                Quaternion.LookRotation(moveDirection).eulerAngles.y,
                0);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // 没有输入时停止移动
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        // 如果被禁锢，处理逃脱机制
        if (isTrapped)
        {
            HandleTrap();
            return; // 跳过正常移动逻辑
        }

        // 应用浮动动画
        if (isFloating)
        {
            ApplyFloatingAnimation();
        }
        else
        {
            // 重置浮动基准位置
            originalPosition = transform.position;
        }
    }

    // 应用浮动动画
    private void ApplyFloatingAnimation()
    {
        // 更新浮动偏移量
        floatOffset += Time.deltaTime * floatSpeed;

        // 计算正弦波浮动值
        float verticalOffset = Mathf.Sin(floatOffset) * floatHeight;

        // 创建目标位置
        Vector3 targetPosition = originalPosition + Vector3.up * verticalOffset;

        // 平滑移动到目标位置
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            floatSmoothness * Time.deltaTime
        );
    }

    public void GetTrapped()
    {
        if (isTrapped) return; // 已经处于禁锢状态则不再处理

        isTrapped = true;
        trapTimer = trapDuration;
        currentPressCount = 0;

        // 立即停止移动
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Debug.Log("玩家被禁锢！");
    }

    private void HandleTrap()
    {
        // 递减计时器
        trapTimer -= Time.deltaTime;

        // 检测逃脱按键（空格键）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentPressCount++;

            // 如果按了足够次数则逃脱
            if (currentPressCount >= escapePresses)
            {
                EscapeTrap();
            }
        }

        // 计时器结束后自动逃脱
        if (trapTimer <= 0)
        {
            EscapeTrap();
        }
    }

    private void EscapeTrap()
    {
        isTrapped = false;
        Debug.Log("玩家逃脱了禁锢！");
    }

    // 调试可视化
    void OnDrawGizmosSelected()
    {
        if (isFloating)
        {
            // 绘制浮动轨迹
            Gizmos.color = Color.green;
            Vector3 startPos = originalPosition - Vector3.up * floatHeight;
            Vector3 endPos = originalPosition + Vector3.up * floatHeight;
            Gizmos.DrawLine(startPos, endPos);

            // 绘制当前浮动位置
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}