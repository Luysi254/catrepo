using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text pineNutText;  // 引用UI文本对象
    private int pineNutsCollected = 0;
    private int totalPineNuts = 5;

    void Start()
    {
        // 初始化文本
        UpdateScoreText();
    }

    public void CollectPineNut()
    {
        pineNutsCollected++;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        pineNutText.text = $"松子: {pineNutsCollected}/{totalPineNuts}";
    }
}

