using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenmanGenerator : MonoBehaviour
{
    public GameObject groundPrefab;   // 拖入你的预制体
    public Transform player;          // 拖入你的主角
    public int initialTiles = 2;      // 初始生成5块
    public float tileWidth = 1664f;     // 一块地面多宽

    private List<GameObject> activeTiles = new List<GameObject>();
    private float nextSpawnX = 0f;

    void Start()
    {
        for (int i = 0; i < initialTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player.position.x + tileWidth * 2f > nextSpawnX)
        {
            SpawnTile();
        }

        // 销毁多余tile，防止卡顿（先检查是否有效）
        if (activeTiles.Count > initialTiles + 2 && activeTiles[0] != null)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }

    }

    void SpawnTile()
    {
        Vector3 spawnPos = new Vector3(nextSpawnX, 164f, 921f);
        GameObject tile = Instantiate(groundPrefab, spawnPos, Quaternion.identity);
        activeTiles.Add(tile);
        nextSpawnX += tileWidth;
    }
}
