using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeedX = 0f; // X方向滚动速度 (在横板中，通常代表道路"移动"方向)
    public float scrollSpeedY = -0.5f; // Y方向滚动速度 (在Plane上，Y轴纹理对应模型Z轴，也就是"向前"方向)
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        rend.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
