using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterGenerator : MonoBehaviour
{
    // 怪物预制体
    public GameObject monsterPrefab;
    // 怪物生成点列表
    public Transform[] spawnPoints;

    private void Start()
    {
        // 为每个生成点生成一个对应的怪物
        GenerateMonstersAtSpawnPoints();
    }

    private void GenerateMonstersAtSpawnPoints()
    {
        // 检查必要的组件
        if (monsterPrefab == null)
        {
            Debug.LogError("怪物预制体未分配！");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("没有设置怪物生成点！");
            return;
        }

        // 为每个生成点创建对应的怪物
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                // 实例化怪物
                GameObject monsterObj = Instantiate(
                    monsterPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

                // 可选：设置怪物父对象为生成点（方便层级管理）
                monsterObj.transform.parent = spawnPoint;

                //Debug.Log($"在 {spawnPoint.name} 生成怪物");
            }
        }
    }

    // 如果需要手动重新生成所有怪物
    public void RegenerateAllMonsters()
    {
        // 先销毁现有的怪物
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && spawnPoint.childCount > 0)
            {
                foreach (Transform child in spawnPoint)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // 重新生成
        GenerateMonstersAtSpawnPoints();
    }

    // 在指定索引的生成点生成怪物
    public void SpawnMonsterAtPoint(int spawnPointIndex)
    {
        if (spawnPointIndex >= 0 && spawnPointIndex < spawnPoints.Length)
        {
            Transform spawnPoint = spawnPoints[spawnPointIndex];
            if (spawnPoint != null)
            {
                GameObject monsterObj = Instantiate(
                    monsterPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );
                monsterObj.transform.parent = spawnPoint;

                Debug.Log($"在生成点 {spawnPointIndex} ({spawnPoint.name}) 生成怪物");
            }
        }
        else
        {
            Debug.LogError($"生成点索引 {spawnPointIndex} 超出范围！");
        }
    }
}