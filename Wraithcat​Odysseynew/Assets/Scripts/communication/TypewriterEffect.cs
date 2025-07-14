using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.1f; 
    private TMP_Text tmpText;
    private string fullText;
    private Coroutine typingCoroutine; // 用于存储协程引用

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        fullText = tmpText.text;
        tmpText.text = ""; // 清空初始文本

        // 只有初始激活的对象才立即开始打字
        if (gameObject.activeInHierarchy)
        {
            StartTyping();
        }
    }

    // 当对象被激活时调用
    void OnEnable()
    {
        // 如果文本内容存在且尚未开始打字
        if (fullText != null && typingCoroutine == null)
        {
            StartTyping();
        }
    }

    // 当对象被禁用时调用
    void OnDisable()
    {
        // 停止正在进行的打字效果
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    // 开始打字效果
    void StartTyping()
    {
        // 确保先停止已有的协程
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            tmpText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null; // 打字完成后清空引用
    }

    // 可选：添加重置方法
    public void ResetTypewriter()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        tmpText.text = "";

        if (gameObject.activeInHierarchy)
        {
            StartTyping();
        }
    }
}