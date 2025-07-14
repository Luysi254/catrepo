using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject groundPrefab;   // 拖入你的预制体
    public Transform player;          // 拖入你的主角
    public int initialTiles = 0;      // 初始生成5块
    public float tileWidth = 1664f;     // 一块地面多宽

    private List<GameObject> activeTiles = new List<GameObject>();
    private float nextSpawnX = 1075f;
 

    void Start()
    {
        for (int i = 0; i < initialTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        // 提前两块距离开始生成，防止主角超过
        if (player.position.x + tileWidth * 2f > nextSpawnX)
        {
            SpawnTile();
        }

        // 销毁多余tile，防止卡顿
        if (activeTiles.Count > initialTiles + 2)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
       

    }

    void SpawnTile()
    {
        Vector3 spawnPos = new Vector3(nextSpawnX, -539f, 741f);
        GameObject tile = Instantiate(groundPrefab, spawnPos, Quaternion.identity);
        activeTiles.Add(tile);
        
        nextSpawnX += tileWidth;
    }

}