using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToBeginning : MonoBehaviour
{
    // 当按钮被点击时调用此方法
    public void LoadBeginningScene()
    {
        // 加载名为"Begining"的场景
        SceneManager.LoadScene("Beginning");

        // 注意：确保场景名称拼写正确（包括大小写）
        // 并且在Build Settings中已经添加了该场景
    }
}
