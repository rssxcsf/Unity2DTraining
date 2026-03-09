using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneEntityConfig", menuName = "Demo/Scene Entity Config")]
public class SceneEntityConfig : ScriptableObject
{
    [Header("玩家生成位置")]
    public Vector3 playerSpawnPosition;

    [Header("敌人刷新点列表（世界坐标）")]
    public Vector3[] enemySpawnPositions;

    [Header("Boss生成位置")]
    public Vector3 bossSpawnPosition;

    [Header("传送门生成位置")]
    public Vector3[] portalSpawnPositions;

    [Header("可破坏物体生成位置")]
    public Vector3[] destructiblesSpawnPositions;

    [Header("敌人波次配置")]
    public List<EnemyWave> enemyWaves;

    [Header("Boss预制体")]
    public GameObject bossPrefab;
}
