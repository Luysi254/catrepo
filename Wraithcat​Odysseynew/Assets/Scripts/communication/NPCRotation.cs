// NPCRotation.cs
using UnityEngine;

public class NPCRotation : MonoBehaviour
{
    public Transform player;          // 玩家角色
    public float detectionDistance = 3f; // 感知距离
    private bool hasRotated = false;  // 是否已旋转

    void Update()
    {
        if (!hasRotated && Vector3.Distance(transform.position, player.position) < detectionDistance)
        {
            // 绕Z轴旋转-90度
            // Rotate -90 degrees around Z axis
            transform.Rotate(0, 0, -90);
            hasRotated = true;

            // 触发对话
            // Trigger dialogue
            // DialogueManager.Instance.StartDialogue();
        }
    }
}