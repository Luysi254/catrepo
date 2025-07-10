using UnityEngine;

public class SocketDetector : MonoBehaviour
{
    [Header("连接配置")]
    public PipePiece parentPipe;
    public bool isConnected;
    public SocketDetector connectedSocket;

    [Header("连接效果")]
    public ParticleSystem connectionFX;
    public AudioClip connectionSound;

    void Start()
    {
        if (isConnected && gameObject.name != "Socket_Start")
        {
            gameObject.name = "Socket_End";
        }
    }

    public void CheckConnection()
    {
        // 清除现有连接
        if (isConnected && connectedSocket != null)
        {
            connectedSocket.isConnected = false;
            connectedSocket.connectedSocket = null;
            isConnected = false;
            connectedSocket = null;
        }

        // 尝试寻找新连接
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach (Collider other in colliders)
        {
            if (other.CompareTag("PipeSocket") && other.gameObject != gameObject)
            {
                SocketDetector otherSocket = other.GetComponent<SocketDetector>();
                if (otherSocket != null && !otherSocket.isConnected)
                {
                    EstablishConnection(otherSocket);
                    return;
                }
            }
        }
    }

    void EstablishConnection(SocketDetector otherSocket)
    {
        // 标记双向连接状态
        isConnected = true;
        otherSocket.isConnected = true;

        // 记录对方插座
        connectedSocket = otherSocket;
        otherSocket.connectedSocket = this;

        // 应用名称匹配
        if (gameObject.name != "Socket_Start")
            gameObject.name = "Socket_End";
        if (otherSocket.gameObject.name != "Socket_Start")
            otherSocket.gameObject.name = "Socket_End";

        PlayConnectionFX();
    }

    void PlayConnectionFX()
    {
        if (connectionFX != null) connectionFX.Play();
        if (connectionSound != null)
            AudioSource.PlayClipAtPoint(connectionSound, transform.position, 0.5f);
    }
}