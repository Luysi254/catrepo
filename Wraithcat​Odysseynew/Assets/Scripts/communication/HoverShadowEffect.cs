using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverShadowEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("绑定Text子对象")]
    public Text targetText; // 绑定你的Text子对象

    private Shadow textShadow; // Shadow组件引用

    void Start()
    {
        // 自动获取Shadow组件
        if (targetText != null)
            textShadow = targetText.GetComponent<Shadow>();

        // 默认隐藏Shadow
        if (textShadow != null)
            textShadow.enabled = false;
    }

    // 鼠标进入时显示Shadow
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (textShadow != null)
            textShadow.enabled = true;
    }

    // 鼠标离开时隐藏Shadow
    public void OnPointerExit(PointerEventData eventData)
    {
        if (textShadow != null)
            textShadow.enabled = false;
    }
}