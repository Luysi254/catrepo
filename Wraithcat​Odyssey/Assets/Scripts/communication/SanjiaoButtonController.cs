using UnityEngine;
using UnityEngine.SceneManagement;

public class SanjiaoButtonController : MonoBehaviour
{

    public GameObject button1NPC;
    public GameObject button2CAT;
    public GameObject button3NPC;

    // 记录点击次数
    private int clickCount = 0;

    // 场景名称
    public string beginningSceneName = "Beginning";

    void Awake()
    {
        // 设置初始状态：button1NPC显示，其他隐藏
        button1NPC.SetActive(true);
        button2CAT.SetActive(false);
        button3NPC.SetActive(false);
    }
    public void OnSanjiaoClicked()
    {
        clickCount++;

        // 三次点击后循环
        if (clickCount > 3)
        {
            clickCount = 1;
        }

        switch (clickCount)
        {
            case 1:
                // 第一次点击：隐藏1和3，显示2
                button1NPC.SetActive(false);
                button3NPC.SetActive(false);
                button2CAT.SetActive(true);
                break;

            case 2:
                // 第二次点击：隐藏2和1，显示3
                button2CAT.SetActive(false);
                button1NPC.SetActive(false);
                button3NPC.SetActive(true);
                break;

            case 3:
                // 第三次点击
                SceneManager.LoadScene(beginningSceneName);
                break;
        }
    }
}