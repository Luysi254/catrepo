using UnityEngine;

public class Camera2DFollow : MonoBehaviour
{
    public Transform target;    // 猫猫
    public float smoothSpeed = 0.1f;
    public Vector3 offset = new Vector3(0, 0, -10);  // 正交相机必须-Z

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = offset.z;  // 固定Z轴
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}