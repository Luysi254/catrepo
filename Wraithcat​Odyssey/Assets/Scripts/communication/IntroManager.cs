using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    public TextMeshProUGUI storyText;    // 中间文字
    public GameObject continueButton;    // 继续按钮
    public GameObject EyeCanvas;
    public EyeOpening eyeOpeningScript;  // EyeOpening脚本

    public string fullText = "你感觉自己的身体很疲惫，忘记了自己是谁，只想极力睁开眼睛....";  // 显示的完整文字
    public float typingSpeed = 0.1f;      // 打字速度

    void Start()
    {
        storyText.text = "";  // 开始时清空
        continueButton.SetActive(false); // 按钮隐藏
        StartCoroutine(TypeSentence());
    }

    IEnumerator TypeSentence()
    {
        foreach (char letter in fullText.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 打字完成后显示按钮
        continueButton.SetActive(true);
    }

    public void OnContinueClicked()
    {
        // 按钮点击时调用EyeOpening
        eyeOpeningScript.StartOpening();
        gameObject.SetActive(false); // 隐藏整个Intro界面
        EyeCanvas.SetActive(false);
    }
}