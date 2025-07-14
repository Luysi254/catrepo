using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemycontrol : MonoBehaviour
{
        [Header("目标设置")]
        public Transform player;

        [Header("追逐设置")]
    public float detectionRange = 50f; // 检测距离
    public float chargeRange = 20f;    // 冲刺范围
    public float chargeSpeed = 35f;    // 冲刺速度
    public float normalSpeed = 15f;    // 普通追逐速度
    public float cooldownAfterCharge = 2f; // 冷却时间
    public float rotationSpeed = 10f;  // 旋转速度


    [Header("组件")]
        public AudioSource chainSound;

        private Rigidbody rb;
        private bool isCharging = false;
        private bool isChasing = false;
        private float chargeTimer = 0f;

        void Start()
        {
            rb = GetComponent<Rigidbody>();

            // 获取AudioSource（如果未设置）
            if (chainSound == null)
                chainSound = GetComponent<AudioSource>();

            // 修复碰撞体
            BoxCollider collider = GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.size = new Vector3(1f, 2f, 1f);
                collider.center = new Vector3(0, 1f, 0);
            }

            // 设置物理约束
            rb.constraints = RigidbodyConstraints.FreezePositionY |
                             RigidbodyConstraints.FreezeRotation;
        }

        void Update()
        {
            // 确保player引用有效
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) player = playerObj.transform;
                else return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Debug.Log($"与玩家距离: {distanceToPlayer}");

            // 检测玩家
            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;

                // 冲刺触发
                if (distanceToPlayer <= chargeRange && !isCharging && chargeTimer <= 0)
                {
                    StartCharge();
                }
            }
            else
            {
                isChasing = false;
            }

            // 冷却处理
            if (chargeTimer > 0) chargeTimer -= Time.deltaTime;

            // 声音控制
            UpdateSound();

            // 状态显示
            UpdateStatusDisplay();
        }

        void FixedUpdate()
        {
            // 确保在追逐状态下才移动
            if (isChasing && player != null)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                if (isCharging)
                {
                    ChargeTowards(direction);
                }
                else
                {
                    NormalChase(direction);
                }
            }
            else
            {
                // 非追逐状态停止移动
                rb.velocity = Vector3.zero;
            }
        }

        private void StartCharge()
        {
            isCharging = true;
            chargeTimer = 0;
            chainSound?.Stop();
        }

        private void ChargeTowards(Vector3 direction)
        {
            rb.velocity = direction * chargeSpeed;
        }

        private void NormalChase(Vector3 direction)
        {
            // 获取安全方向（避免障碍）
            Vector3 safeDirection = GetSafeDirection(direction);

            // 更新旋转
            if (safeDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(safeDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                   rotationSpeed * Time.fixedDeltaTime);
            }

            // 向目标方向移动
            rb.velocity = safeDirection * normalSpeed;
        }

        private Vector3 GetSafeDirection(Vector3 targetDirection)
        {
            // 简单的障碍检测与回避
            RaycastHit hit;
            Vector3[] testDirections = new Vector3[]
            {
            targetDirection,
            Quaternion.Euler(0, 30, 0) * targetDirection,
            Quaternion.Euler(0, -30, 0) * targetDirection,
            Quaternion.Euler(0, 60, 0) * targetDirection,
            Quaternion.Euler(0, -60, 0) * targetDirection
            };

            foreach (Vector3 dir in testDirections)
            {
                if (!Physics.Raycast(transform.position, dir, out hit, 3f))
                {
                    return dir.normalized;
                }
            }

            // 所有方向都受阻，转向后撤
            return -targetDirection;
        }

        void OnCollisionEnter(Collision collision)
        {
            // 冲刺状态下碰撞玩家
            if (isCharging && collision.gameObject.CompareTag("Player"))
            {
                CatController cat = collision.gameObject.GetComponent<CatController>();
                if (cat != null)
                {
                    cat.GetTrapped();
                    StopCharge();
                }
            }
            // 碰撞墙壁结束冲刺
            else if (isCharging && collision.gameObject.CompareTag("Wall"))
            {
                StopCharge();
            }
        }

        private void StopCharge()
        {
            isCharging = false;
            rb.velocity = Vector3.zero;
            chargeTimer = cooldownAfterCharge;
        }

        private void UpdateSound()
        {
            if (isChasing && !chainSound.isPlaying && !isCharging)
            {
                chainSound.Play();
            }
            else if ((!isChasing || isCharging) && chainSound.isPlaying)
            {
                chainSound.Stop();
            }
        }

        private void UpdateStatusDisplay()
        {
            string state = isChasing ? (isCharging ? "冲刺" : "追逐") : "空闲";
            Debug.Log($"敌人状态: {state}, 冷却: {chargeTimer.ToString("F2")}");
        }

        // 调试可视化
        void OnDrawGizmosSelected()
        {
            // 检测范围
            Gizmos.color = new Color(1, 1, 0, 0.3f); // 半透明黄色
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // 冲刺范围
            Gizmos.color = new Color(1, 0, 0, 0.3f); // 半透明红色
            Gizmos.DrawWireSphere(transform.position, chargeRange);

            // 到玩家的线
            if (player != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }

