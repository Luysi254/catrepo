using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"钥匙已创建: {name}");
        Debug.Log($"位置: {transform.position}, 旋转: {transform.rotation.eulerAngles}");

      
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("钥匙被收集!");

            if (GameManager.instance != null)
            {
                GameManager.instance.ObtainKey();
            }

            Destroy(gameObject);
        }
    }
}