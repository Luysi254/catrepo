// NPCFloating.cs
using UnityEngine;

public class NPCFloating : MonoBehaviour
{
    public float floatHeight = 0.5f;  // 浮动高度
    public float floatSpeed = 1f;     // 浮动速度

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 使用正弦函数创建上下浮动效果
        // Using sine function to create floating effect
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}