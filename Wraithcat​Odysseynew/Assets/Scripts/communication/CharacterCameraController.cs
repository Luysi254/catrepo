using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [Header("必填项")]
    public Transform cat; // 拖入Hierarchy中的猫对象

    [Header("跟随设置")]
    public float followSpeed = 5f;    // 跟随响应速度
    public float heightOffset = 0f;   // 垂直偏移（保持截图Y=0）
    public float distance = 0.66f;   // Z轴距离（匹配截图Z=0.66）

    void LateUpdate()
    {
        if (cat == null) return;

        // 水平跟随（仅同步X轴，固定Y/Z）
        Vector3 targetPos = new Vector3(
            cat.position.x + 1.8f,    // 保持截图X偏移1.8
            heightOffset,            // 固定Y轴
            -distance                // 固定Z轴距离
        );

        // 平滑移动
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

    }
}
