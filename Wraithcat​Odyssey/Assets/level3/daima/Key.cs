using UnityEngine;

public class Key : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("钥匙被收集!");
            GameManager.instance?.ObtainKey(); // 直接调用 GameManager
            Destroy(gameObject);
        }
    }
}