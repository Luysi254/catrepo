using UnityEngine;

[RequireComponent(typeof(Camera))]
public class camerafollow : MonoBehaviour
{
    [Header("基础跟随设置")]
    [Tooltip("要跟随的目标对象（通常是玩家角色）")]
    public Transform target;

    [Tooltip("跟随平滑度（值越大跟随越紧）")]
    [Range(1f, 20f)] public float smoothSpeed = 5f;

    [Tooltip("摄像机相对于目标的偏移量")]
    public Vector3 offset = new Vector3(0f, 3f, -5f);

    [Header("跟随限制")]
    [Tooltip("是否跟随X轴移动")] public bool followX = true;
    [Tooltip("是否跟随Y轴移动")] public bool followY = true;
    [Tooltip("是否跟随Z轴移动")] public bool followZ = true;

    [Header("旋转设置")]
    [Tooltip("是否随玩家旋转")] public bool rotateWithTarget = true;

    [Tooltip("旋转平滑度")]
    [Range(1f, 20f)] public float rotationSmoothness = 10f;

    [Header("碰撞避免")]
    [Tooltip("摄像机碰撞检测层")]
    public LayerMask collisionMask;

    [Tooltip("避免穿墙的距离")]
    [Range(0.1f, 1f)] public float wallClipDistance = 0.5f;

    [Header("高级距离控制")]
    [Tooltip("启用动态距离控制")] public bool enableDistanceControl = false;

    [Tooltip("最小跟随距离")]
    [Min(1f)] public float minDistance = 3f;

    [Tooltip("最大跟随距离")]
    [Min(5f)] public float maxDistance = 10f;

    [Tooltip("缩放灵敏度")]
    [Range(0.1f, 2f)] public float zoomSensitivity = 1f;

    [Header("第三人称视角")]
    [Tooltip("启用第三人称视角控制")] public bool enableThirdPersonView = false;

    [Tooltip("视角旋转速度")]
    [Range(0.1f, 5f)] public float rotationSpeed = 1f;

    // 私有变量
    private Vector3 desiredPosition;    // 期望的摄像机位置
    private Vector3 adjustedPosition;   // 调整后的位置（避免穿墙）
    private float currentDistance;      // 当前跟随距离
    private float currentYaw;           // 当前偏航角（用于第三人称视角）
    private Camera cam;                 // 摄像机组件
    private Vector3 lastTargetPosition; // 目标上一帧的位置

    void Start()
    {
        // 获取摄像机组件
        cam = GetComponent<Camera>();

        // 自动查找玩家对象
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("自动找到玩家目标: " + target.name);
            }
            else
            {
                Debug.LogWarning("摄像机未找到玩家对象，请手动指定目标");
            }
        }

        // 初始化距离
        currentDistance = Mathf.Abs(offset.z);

        // 设置初始位置
        if (target != null)
        {
            UpdateDesiredPosition();
            transform.position = desiredPosition;
            transform.LookAt(target.position);
            lastTargetPosition = target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 更新期望位置
        UpdateDesiredPosition();

        // 避免穿墙
        adjustedPosition = AdjustForWallCollision(desiredPosition);

        // 平滑移动摄像机
        transform.position = Vector3.Lerp(
            transform.position,
            adjustedPosition,
            smoothSpeed * Time.deltaTime
        );

        // 平滑旋转摄像机
        if (rotateWithTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothness * Time.deltaTime
            );
        }

        // 保存目标位置用于下一帧
        lastTargetPosition = target.position;
    }

    void Update()
    {
        if (target == null) return;

        // 处理距离控制
        if (enableDistanceControl)
        {
            HandleDistanceControl();
        }

        // 处理第三人称视角控制
        if (enableThirdPersonView)
        {
            HandleThirdPersonView();
        }
    }

    // 更新期望位置
    private void UpdateDesiredPosition()
    {
        // 计算基本偏移
        Vector3 baseOffset = new Vector3(
            followX ? offset.x : 0f,
            followY ? offset.y : 0f,
            followZ ? offset.z : 0f
        );

        // 应用距离控制
        if (enableDistanceControl)
        {
            baseOffset.z = -currentDistance;
        }

        // 应用第三人称视角旋转
        if (enableThirdPersonView)
        {
            Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
            desiredPosition = target.position + rotation * baseOffset;
        }
        else
        {
            // 标准跟随
            desiredPosition = target.position +
                              target.right * baseOffset.x +
                              target.up * baseOffset.y +
                              target.forward * baseOffset.z;
        }
    }

    // 处理距离控制
    private void HandleDistanceControl()
    {
        // 鼠标滚轮控制距离
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            currentDistance = Mathf.Clamp(
                currentDistance - scroll * zoomSensitivity,
                minDistance,
                maxDistance
            );
        }
    }

    // 处理第三人称视角控制
    private void HandleThirdPersonView()
    {
        // 鼠标右键旋转视角
        if (Input.GetMouseButton(1))
        {
            currentYaw += Input.GetAxis("Mouse X") * rotationSpeed;
        }
    }

    // 避免穿墙的方法
    private Vector3 AdjustForWallCollision(Vector3 targetPosition)
    {
        // 从玩家到摄像机的方向
        Vector3 direction = targetPosition - target.position;
        float distance = direction.magnitude;
        direction.Normalize();

        // 使用球体投射检测碰撞
        RaycastHit hit;
        if (Physics.SphereCast(target.position, 0.2f, direction, out hit, distance, collisionMask))
        {
            // 如果碰到墙壁，调整摄像机位置避免穿墙
            return target.position + direction * (hit.distance - wallClipDistance);
        }

        return targetPosition;
    }

    // 显示调试信息
    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // 绘制期望位置
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(desiredPosition, 0.3f);

        // 绘制实际位置
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        // 绘制避免穿墙区域
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(adjustedPosition, 0.15f);

        // 绘制摄像机到目标的线
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(target.position, transform.position);

        // 绘制碰撞检测区域
        Gizmos.color = new Color(1, 0.5f, 0, 0.5f);
        Vector3 direction = desiredPosition - target.position;
        Gizmos.DrawLine(target.position, target.position + direction.normalized * direction.magnitude);
        Gizmos.DrawWireSphere(target.position, 0.2f);
    }

    // 公共方法：设置新目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    // 公共方法：重置摄像机位置
    public void ResetCameraPosition()
    {
        if (target != null)
        {
            UpdateDesiredPosition();
            transform.position = desiredPosition;
            transform.LookAt(target.position);
        }
    }

    // 公共方法：切换跟随模式
    public void ToggleFollowMode(bool followX, bool followY, bool followZ)
    {
        this.followX = followX;
        this.followY = followY;
        this.followZ = followZ;
    }

    // 公共方法：设置偏移量
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        if (enableDistanceControl)
        {
            currentDistance = Mathf.Abs(offset.z);
        }
    }
}