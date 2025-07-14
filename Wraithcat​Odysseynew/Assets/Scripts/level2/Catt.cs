using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Catt : MonoBehaviour
{
    // 坠落高度阈值
    public float fallDeathHeight = -15f;
    // 移动参数
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;
    public LayerMask groundLayer;

    // 引用
    public Transform headPosition;
    public GameObject branchPrefab;
    public float branchYOffset = 0.5f;
    public float groundCheckDistance = 0.5f;
    public Transform footPosition;

    // 状态
    private GameObject carriedBranch;
    private bool isBranchForm = false;
    private Rigidbody rb;
    private CapsuleCollider col;
    private bool isGrounded;

    private List<GameObject> highlightedBranches = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        if (rb != null) rb.useGravity = false;

        // 修正初始旋转
        transform.rotation = Quaternion.Euler(-90f, 0, 0);
    }

    void Update()
    {
        CheckGroundStatus();
        if (!isBranchForm)
        {
            HandleMovement(); // 修复移动逻辑
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (carriedBranch == null && highlightedBranches.Count > 0)
            {
                // 拾取第一个高亮的树枝
                PickupBranch(highlightedBranches[0]);
            }
            else if (carriedBranch != null)
            {
                DropBranch();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ToggleForm();
        }
        CheckFallDeath();
    }
    void CheckFallDeath()
    {
        if (transform.position.y < fallDeathHeight)
        {
            ReloadScene();
        }
    }

    // 新增方法：重新加载当前场景
    void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        // 或者直接使用场景名称（更稳定）
        // SceneManager.LoadScene("你的场景名称");
    }
    void CheckGroundStatus()
    {
        // 1. 检测标准地面（草地）- 修改为动态检测距离
        float dynamicCheckDistance = groundCheckDistance + Mathf.Abs(transform.position.y);
        bool isOnGround = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            dynamicCheckDistance,  // 动态计算检测距离
            groundLayer
        );

        // 2. 检测已放置的桥梁（完全保留原有逻辑）
        bool isOnBridge = false;
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var collider in nearbyColliders)
        {
            BranchInfo branchInfo = collider.GetComponent<BranchInfo>();
            if (branchInfo != null && branchInfo.isPlaced)
            {
                isOnBridge = true;
                break;
            }
        }

        // 3. 合并结果（不变）
        isGrounded = isOnGround || isOnBridge;

        // 4. 控制重力（不变）
        if (rb != null)
        {
            rb.useGravity = !isGrounded;
            if (isGrounded && rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }

        // 5. 新增：强制贴地（仅在检测到地面时生效）
        if (isOnGround)
        {
            RaycastHit groundHit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                              Vector3.down,
                              out groundHit,
                              dynamicCheckDistance,
                              groundLayer))
            {
                float targetY = groundHit.point.y + col.height * 0.5f;
                transform.position = new Vector3(
                    transform.position.x,
                    targetY,
                    transform.position.z
                );
            }
        }
    }

    // 修复移动逻辑（关键修改）
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 1. 获取相机相对方向（忽略Y轴）
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 2. 计算相机空间的移动方向
        Vector3 cameraRelativeDir = (cameraForward * vertical) + (cameraRight * horizontal);
        if (cameraRelativeDir == Vector3.zero) return;

        // 3. 将相机方向转换为世界方向
        float inputAngle = Mathf.Atan2(cameraRelativeDir.x, cameraRelativeDir.z) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(inputAngle / 90) * 90; // 对齐到0/90/180/270

        // 4. 计算实际移动方向（基于对齐后的角度）
        Vector3 worldMoveDir = Quaternion.Euler(0, snappedAngle, 0) * Vector3.forward;

        // 5. 应用移动和旋转
        transform.position += worldMoveDir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(-90f, snappedAngle, 0);
    }


    // 切换形态
    void ToggleForm()
    {
        isBranchForm = !isBranchForm;

        // 隐藏/显示小猫模型
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = !isBranchForm;
        }

        // 树枝形态时禁用碰撞体和移动
        col.enabled = !isBranchForm;
        rb.isKinematic = isBranchForm;
    }

    // 拾取树枝逻辑
    void PickupBranch(GameObject branch)
    {
        ResetAllHighlightedBranches();
        // 生成携带的树枝，挂在头顶
        carriedBranch = Instantiate(branchPrefab, headPosition.position, Quaternion.identity);
        carriedBranch.transform.SetParent(headPosition);
        carriedBranch.transform.localRotation = Quaternion.Euler(0, 90, 90);
        carriedBranch.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // 移除场景中的原树枝
        Destroy(branch);

        Debug.Log("拾取树枝成功！");
    }
    void ResetAllHighlightedBranches()
    {
        foreach (var branch in highlightedBranches)
        {
            if (branch != null)
            {
                BranchData data = branch.GetComponent<BranchInfo>().data;
                branch.transform.position = data.originalPosition;
                branch.GetComponent<Renderer>().material.color = data.originalColor;
            }
        }
        highlightedBranches.Clear();
    }

    // 放下树枝逻辑
    void DropBranch()
    {
        if (carriedBranch != null)
        {
            // 检查当前位置是否在Grass区域内
            Collider[] areaColliders = Physics.OverlapSphere(footPosition.position, 0.5f);
            bool isInGrassArea = false;

            foreach (var collider in areaColliders)
            {
                if (collider.CompareTag("Grass") && collider.gameObject.name.Contains("GrassArea"))
                {
                    isInGrassArea = true;
                    break;
                }
            }

            if (isInGrassArea)
            {
                Debug.Log("此处不可搭建桥梁！");
                // 可以在这里添加UI提示，比如：
                // ShowNotification("草地区域不能搭建桥梁！");
                return; // 直接返回，不执行后续搭建逻辑
            }

            // 使用与拾取时相同的旋转
            Quaternion bridgeRotation = carriedBranch.transform.rotation;

            Vector3 bridgePosition = footPosition.position;
            bridgePosition.y -= 0.2f; // 微调高度

            // 可选：添加地面检测确保桥梁放在地面上
            RaycastHit hit;
            if (Physics.Raycast(bridgePosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, groundLayer))
            {
                bridgePosition.y = hit.point.y;
            }

            // 生成桥梁
            GameObject bridge = Instantiate(
                carriedBranch,
                bridgePosition,
                bridgeRotation  // 使用一致的旋转
            );

            Collider bridgeCollider = bridge.GetComponent<Collider>();
            if (bridgeCollider != null)
            {
                bridgeCollider.isTrigger = false; // 关键修改：关闭Trigger
            }

            // 保持原有标记逻辑
            bridge.tag = "Untagged";
            BranchInfo bridgeInfo = bridge.GetComponent<BranchInfo>() ?? bridge.AddComponent<BranchInfo>();
            bridgeInfo.isPlaced = true;

            // 重置桥的父对象和大小
            bridge.transform.SetParent(null);
            bridge.transform.localScale = new Vector3(1f, 1f, 1.2f);

            // 移除携带的树枝
            Destroy(carriedBranch);
            carriedBranch = null;

            Debug.Log("放下树枝，生成桥！");
        }
    }

    // 检测是否靠近树枝，高亮提示
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Branch") && carriedBranch == null)
        {
            // 确保 BranchInfo 组件存在
            BranchInfo branchInfo = other.GetComponent<BranchInfo>();
            if (branchInfo == null || !branchInfo.isPlaced)
            {
                if (branchInfo == null) branchInfo = other.gameObject.AddComponent<BranchInfo>();

                // 记录原始状态
                branchInfo.data = new BranchData
                {
                    originalPosition = other.transform.position,
                    originalColor = other.GetComponent<Renderer>().material.color
                };

                // 高亮处理
                other.transform.Translate(Vector3.up * branchYOffset);
                other.GetComponent<Renderer>().material.color = Color.yellow;

                highlightedBranches.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Branch") && highlightedBranches.Contains(other.gameObject))
        {
            // 复位
            BranchData data = other.GetComponent<BranchInfo>().data;
            other.transform.position = data.originalPosition;
            other.GetComponent<Renderer>().material.color = data.originalColor;

            highlightedBranches.Remove(other.gameObject);
        }
    }

}