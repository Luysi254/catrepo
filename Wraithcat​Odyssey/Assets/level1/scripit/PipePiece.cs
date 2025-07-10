using UnityEngine;
using System.Collections.Generic;

public class PipePiece : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationDuration = 0.5f;
    public Vector3[] rotationAxes = { Vector3.up, Vector3.right, Vector3.forward };

    [Header("钥匙设置")]
    public Vector3 keyPosition = new Vector3(-2f, 0.5f, 0f);
    public Vector3 keyRotation = new Vector3(0f, 180f, -90f);
    public Vector3 keyScale = new Vector3(0.2f, 0.2f, 0.2f);

    [Header("管道类型")]
    public PipeType pipeType = PipeType.Middle;
    public enum PipeType { Entrance, Exit, Middle }

    [Header("连接检测")]
    public GameObject keyPrefab;
    public bool keySpawned = false;

    // 私有变量
    private bool isRotating;
    private Quaternion targetRotation;
    private float rotationProgress;
    private int currentAxisIndex = 0;

    [System.NonSerialized] public List<SocketDetector> socketCache = new List<SocketDetector>();
    private bool canRotate = true;
    private int currentRotationIndex = 0;

    void Start()
    {
        CacheSockets();

        if (pipeType == PipeType.Entrance)
        {
            SocketDetector startSocket = GetComponentInChildren<SocketDetector>();
            if (startSocket != null)
            {
                startSocket.isConnected = true;
                startSocket.gameObject.name = "Socket_Start";
            }
            canRotate = false;
            SnapToNearestPosition();
        }
        else if (pipeType == PipeType.Exit)
        {
            canRotate = false;
            SnapToNearestPosition();
            EnsureSocketNaming();
        }
    }

    void EnsureSocketNaming()
    {
        foreach (SocketDetector socket in socketCache)
        {
            if (socket.gameObject.name != "Socket_End")
            {
                socket.gameObject.name = "Socket_End";
            }
        }
    }

    void CacheSockets()
    {
        socketCache.Clear();
        foreach (SocketDetector socket in GetComponentsInChildren<SocketDetector>())
        {
            socketCache.Add(socket);
            socket.parentPipe = this;
        }
    }

    void OnMouseDown()
    {
        if (!canRotate) return;

        if (IsMouseOverThisPipe() && canRotate && !isRotating)
        {
            StartRotation();
        }
    }

    bool IsMouseOverThisPipe()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }
        return false;
    }

    void StartRotation()
    {
        isRotating = true;
        rotationProgress = 0f;
        canRotate = false;

        Quaternion rotationStep = Quaternion.AngleAxis(90, rotationAxes[currentAxisIndex]);
        targetRotation = rotationStep * transform.rotation;
        currentAxisIndex = (currentAxisIndex + 1) % rotationAxes.Length;
        currentRotationIndex = (currentRotationIndex + 1) % 4;
    }

    void Update()
    {
        if (isRotating)
        {
            rotationProgress += Time.deltaTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Mathf.SmoothStep(0, 1, rotationProgress)
            );

            if (rotationProgress >= 1f)
            {
                isRotating = false;
                OnRotationComplete();
            }
        }
    }

    void OnRotationComplete()
    {
        canRotate = true;
        SnapToNearestPosition();
        UpdateConnectionStatus();

        if (PipeLevelManager.Instance != null)
            PipeLevelManager.Instance.CheckPuzzleProgress();
    }

    void SnapToNearestPosition()
    {
        Vector3 euler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(
            Mathf.Round(euler.x / 90) * 90,
            Mathf.Round(euler.y / 90) * 90,
            Mathf.Round(euler.z / 90) * 90
        );
    }

    public void UpdateConnectionStatus()
    {
        foreach (SocketDetector socket in socketCache)
        {
            socket.CheckConnection();
        }
    }

    public void SpawnKey()
    {
        if (keyPrefab == null || keySpawned)
        {
            Debug.LogWarning($"钥匙生成失败: 预制体={keyPrefab != null}, 已生成={keySpawned}");
            return;
        }

        GameObject key = Instantiate(
            keyPrefab,
            keyPosition,
            Quaternion.Euler(keyRotation)
        );

        key.transform.localScale = keyScale;
        key.SetActive(true);
        keySpawned = true;

        yaoshi keyCollectible = key.GetComponent<yaoshi>();
        if (keyCollectible != null)
        {
            keyCollectible.keyType = yaoshi.KeyType.PipePuzzle;
        }
    }

    public bool AreAllSocketsConnected()
    {
        foreach (SocketDetector socket in socketCache)
        {
            Collider[] colliders = Physics.OverlapSphere(
                socket.transform.position,
                0.3f,
                LayerMask.GetMask("PipeSocket"));

            if (!socket.isConnected && colliders.Length == 0)
            {
                Debug.LogWarning($"{socket.name} 未检测到附近插座");
                return false;
            }
        }
        return true;
    }

    public int GetCurrentRotationIndex() => currentRotationIndex;

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (SocketDetector socket in socketCache)
        {
            if (socket != null)
            {
                Gizmos.DrawSphere(socket.transform.position, 0.05f);
                Gizmos.DrawLine(transform.position, socket.transform.position);
            }
        }
    }
#endif
}