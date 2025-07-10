using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGet : MonoBehaviour
{
    // 漂浮参数
    public float floatHeight = 0.5f;    // 上下浮动高度
    public float floatSpeed = 1f;       // 浮动速度
    private Vector3 startPos;          // 初始位置

    // 旋转参数
    public float rotateSpeed = 90f;    // 旋转速度（度/秒）

    void Start()
    {
        startPos = transform.position; // 记录初始位置

    }

    void Update()
    {
        // 上下漂浮效果（使用正弦函数）
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // 绕世界Y轴旋转（忽略物体自身旋转）
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
