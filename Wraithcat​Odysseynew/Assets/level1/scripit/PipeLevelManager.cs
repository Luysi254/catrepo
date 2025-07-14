using UnityEngine;
using System.Collections.Generic;

public class PipeLevelManager : MonoBehaviour
{
    public static PipeLevelManager Instance { get; private set; }

    public List<PipePiece> allPipes = new List<PipePiece>();
    public AudioClip successSound;

    // 各管道正确的旋转角度
    private Dictionary<string, Vector3> correctRotations = new Dictionary<string, Vector3>()
    {
        {"Line003", new Vector3(0, 180, 180)},
        {"Line004", new Vector3(0, 0, 0)},
        {"Line009", new Vector3(0, 180, 0)},
        {"Line010", new Vector3(90, 270, 0)},
        {"Line011", new Vector3(90, 270, 0)},
        {"Line012", new Vector3(0, 180, 90)}
    };

    // 调试面板相关变量
    private bool showDebugPanel = true;
    private Vector2 scrollPosition;
    private GUIStyle debugStyle;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CollectAllPipes();
        InitializeDebugStyle();
    }

    void InitializeDebugStyle()
    {
        debugStyle = new GUIStyle();
        debugStyle.fontSize = 14;
        debugStyle.normal.textColor = Color.white;
    }

    void CollectAllPipes()
    {
        allPipes.Clear();

        string[] pipeNames = {
            "Line002", "Line003", "Line004", "Line009",
            "Line010", "Line011", "Line012"
        };

        foreach (string pipeName in pipeNames)
        {
            GameObject pipeObj = GameObject.Find(pipeName);
            if (pipeObj != null)
            {
                PipePiece pipe = pipeObj.GetComponent<PipePiece>();
                if (pipe != null)
                {
                    allPipes.Add(pipe);

                    // 自动设置管道类型
                    if (pipeName == "Line002")
                        pipe.pipeType = PipePiece.PipeType.Entrance;
                    else if (pipeName == "Line012")
                        pipe.pipeType = PipePiece.PipeType.Exit;
                    else
                        pipe.pipeType = PipePiece.PipeType.Middle;
                }
            }
        }
    }

    public void CheckPuzzleProgress()
    {
        if (IsPuzzleComplete())
            OnPuzzleComplete();
    }

    public bool IsPuzzleComplete()
    {
        // 检查所有管道是否连接
        if (!AreAllPipesConnected())
        {
            Debug.Log("管道未完全连接");
            return false;
        }

        // 检查每个管道的旋转角度是否正确
        foreach (var pipe in allPipes)
        {
            string pipeName = pipe.gameObject.name;

            // 入口管道不需要检查旋转
            if (pipe.pipeType == PipePiece.PipeType.Entrance)
                continue;

            // 检查是否有该管道的正确旋转记录
            if (correctRotations.ContainsKey(pipeName))
            {
                Vector3 currentRotation = pipe.transform.eulerAngles;
                Vector3 correctRotation = correctRotations[pipeName];

                // 由于Unity的欧拉角在0-360之间，需要进行角度转换比较
                if (!ApproximatelyEqual(currentRotation, correctRotation))
                {
                    Debug.Log($"管道 {pipeName} 旋转错误: {currentRotation} != {correctRotation}");
                    return false;
                }
            }
        }

        return true;
    }

    // 比较两个欧拉角是否近似相等（考虑360度循环）
    private bool ApproximatelyEqual(Vector3 a, Vector3 b)
    {
        float threshold = 1.5f; // 允许1.5度误差
        return Mathf.Abs(NormalizeAngle(a.x - b.x)) < threshold &&
               Mathf.Abs(NormalizeAngle(a.y - b.y)) < threshold &&
               Mathf.Abs(NormalizeAngle(a.z - b.z)) < threshold;
    }

    // 增强版角度归一化
    private float NormalizeAngle(float angle)
    {
        angle %= 360;
        return angle > 180 ? angle - 360 : angle;
    }

    // 新增：检查所有管道是否连接
    private bool AreAllPipesConnected()
    {
        foreach (PipePiece pipe in allPipes)
        {
            if (!pipe.AreAllSocketsConnected())
            {
                Debug.LogWarning($"管道 {pipe.gameObject.name} 未完全连接");
                return false;
            }
        }
        return true;
    }

    void OnPuzzleComplete()
    {
        if (successSound != null)
            AudioSource.PlayClipAtPoint(successSound, Vector3.zero, 1.0f);

        PipePiece exitPipe = allPipes.Find(p => p.pipeType == PipePiece.PipeType.Exit);
        if (exitPipe != null)
        {
            exitPipe.SpawnKey();
        }
        else
        {
            Debug.LogError("拼图完成但找不到出口管道");
        }
    }

    // ====================== 调试面板实现 ======================
    /* void OnGUI()
     {
         if (!showDebugPanel) return;

         // 调试面板背景
         GUI.Box(new Rect(10, 10, 400, 250), "管道拼图调试面板");

         // 滚动视图
         scrollPosition = GUI.BeginScrollView(
             new Rect(15, 40, 390, 200),
             scrollPosition,
             new Rect(0, 0, 370, allPipes.Count * 70)
         );

         // 管道状态显示
         for (int i = 0; i < allPipes.Count; i++)
         {
             PipePiece pipe = allPipes[i];
             float yPos = i * 70;

             // 管道标题
             GUI.Label(new Rect(10, yPos, 200, 30),
                 $"{i + 1}. {pipe.gameObject.name} ({pipe.pipeType})", debugStyle);

             // 管道状态
             string state = GetPipeState(pipe);
             GUI.Label(new Rect(220, yPos, 150, 30), state, debugStyle);

             // 连接状态
             GUI.Label(new Rect(10, yPos + 25, 350, 30),
                 $"连接状态: {GetConnectionStatus(pipe)}", debugStyle);

             // 旋转状态
             if (correctRotations.ContainsKey(pipe.gameObject.name))
             {
                 Vector3 currentRotation = pipe.transform.eulerAngles;
                 Vector3 correctRotation = correctRotations[pipe.gameObject.name];
                 bool isCorrect = ApproximatelyEqual(currentRotation, correctRotation);

                 GUI.Label(new Rect(10, yPos + 45, 350, 30),
                     $"旋转状态: {currentRotation} / 正确: {correctRotation} <color={(isCorrect ? "green" : "red")}>{(isCorrect ? "✔" : "✖")}</color>", debugStyle);
             }
         }

         GUI.EndScrollView();

         // 整体状态
         string puzzleState = IsPuzzleComplete() ?
             "<color=green>拼图已完成</color>" :
             "<color=red>拼图未完成</color>";
         GUI.Label(new Rect(15, 250, 380, 30), puzzleState, debugStyle);

         // 显示/隐藏调试面板按钮
         if (GUI.Button(new Rect(10, Screen.height - 40, 150, 30),
             showDebugPanel ? "隐藏调试面板" : "显示调试面板"))
         {
             showDebugPanel = !showDebugPanel;
         }
     }

     // 获取管道状态文本
     private string GetPipeState(PipePiece pipe)
     {
         switch (pipe.pipeType)
         {
             case PipePiece.PipeType.Entrance:
                 return "<color=cyan>入口管道</color>";

             case PipePiece.PipeType.Exit:
                 bool exitCorrect = correctRotations.ContainsKey(pipe.gameObject.name) &&
                                  ApproximatelyEqual(pipe.transform.eulerAngles, correctRotations[pipe.gameObject.name]);
                 return $"<color={(exitCorrect ? "green" : "red")}>出口管道</color>";

             default:
                 bool connected = pipe.AreAllSocketsConnected();
                 bool rotationCorrect = correctRotations.ContainsKey(pipe.gameObject.name) &&
                                      ApproximatelyEqual(pipe.transform.eulerAngles, correctRotations[pipe.gameObject.name]);

                 if (!connected) return "<color=orange>中间管道(未连接)</color>";
                 if (!rotationCorrect) return "<color=yellow>中间管道(旋转错误)</color>";
                 return "<color=green>中间管道</color>";
         }
     }

     // 获取连接状态文本
     private string GetConnectionStatus(PipePiece pipe)
     {
         if (pipe.socketCache == null || pipe.socketCache.Count == 0)
             return "<color=red>没有检测点</color>";

         List<string> statuses = new List<string>();
         foreach (SocketDetector socket in pipe.socketCache)
         {
             if (pipe.pipeType == PipePiece.PipeType.Entrance &&
                 socket.gameObject.name == "Socket_Start")
             {
                 statuses.Add("<color=cyan>起点(固定)</color>");
                 continue;
             }

             statuses.Add(socket.isConnected ?
                 $"<color=green>{socket.name}</color>" :
                 $"<color=red>{socket.name}断开</color>");
         }

         return string.Join(" | ", statuses);
     }
     // ====================== 调试面板结束 ======================*/
}