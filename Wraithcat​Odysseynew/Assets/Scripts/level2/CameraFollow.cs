using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target; // 拖入小猫的Transform

    [Header("相机参数")]
    public float distance = 5.0f; // 相机与目标的距离
    public float height = 2.0f;   // 相机高度偏移
    public float rotationSpeed = 3.0f; // 鼠标旋转速度

    [Header("视角限制")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 80f;

    private float currentX = 0f;
    private float currentY = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // 鼠标输入控制旋转
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // 计算相机位置
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = target.position + rotation * dir + Vector3.up * height;

        // 始终看向目标
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }
}