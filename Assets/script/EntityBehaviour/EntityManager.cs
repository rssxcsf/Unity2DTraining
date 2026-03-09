using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { get; private set; }

    [Header("玩家预制体")]
    [SerializeField] private Entity playerPrefab;

    [SerializeField] private GameObject Portal;

    private string currentSceneName;
    private SceneEntityConfig currentConfig;
    private Entity playerInstance;
    private int currentWaveIndex;
    private int activeEnemyCount;
    private bool portalTriggered;

    // 对象池字典
    private Dictionary<GameObject, Queue<Entity>> enemyPool = new Dictionary<GameObject, Queue<Entity>>();
    private Dictionary<GameObject, Queue<Entity>> desPool = new Dictionary<GameObject, Queue<Entity>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == currentSceneName)
        {
            CleanupAllEntities();
            StopAllCoroutines();
        }
    }

    private void CleanupAllEntities()
    {
        // 清理玩家
        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
            playerInstance = null;
        }

        // 清理所有对象池中的敌人
        foreach (var pool in enemyPool.Values)
        {
            while (pool.Count > 0)
            {
                Entity enemy = pool.Dequeue();
                if (enemy != null) Destroy(enemy.gameObject);
            }
        }
        enemyPool.Clear();

        // 清理所有活跃的敌人和传送门
        CleanupActiveEnemies();
        CleanupPortals();
    }
    private void CleanupActiveEnemies()
    {
        // 查找并销毁所有敌人
        Enemy[] activeEnemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in activeEnemies)
        {
            Destroy(enemy.gameObject);
        }
        activeEnemyCount = 0;
    }

    private void CleanupPortals()
    {
        // 查找并销毁所有传送门
        Portal[] portals = FindObjectsOfType<Portal>();
        foreach (Portal portal in portals)
        {
            Destroy(portal.gameObject);
        }
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        InitializeForScene();
    }

    private void InitializeForScene()
    {
        CleanupPreviousScene();
        LoadSceneConfig();
        if (currentConfig == null) return;
        SpawnPlayer();
        PreloadEnemies();
        PreloadDestructibles();
        StartCoroutine(StartNextWave());
    }

    private void CleanupPreviousScene()
    {
        currentWaveIndex = 0;
        activeEnemyCount = 0;

        // 清理对象池
        foreach (var pool in enemyPool.Values)
        {
            while (pool.Count > 0)
            {
                Entity enemy = pool.Dequeue();
                if (enemy != null) Destroy(enemy.gameObject);
            }
        }
        enemyPool.Clear();

        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
        }
    }

    private void LoadSceneConfig()
    {
        string configPath = $"Data/SceneConfigs/{currentSceneName}";
        currentConfig = Resources.Load<SceneEntityConfig>(configPath);

        if (currentConfig == null)
        {
            Debug.LogWarning($"未找到场景配置: {configPath}");
            return;
        }
    }

    private void PreloadEnemies()
    {
        if (currentConfig == null) return;

        Dictionary<GameObject, int> maxEnemies = new Dictionary<GameObject, int>();

        // 计算每个敌人类型需要的最大数量
        foreach (EnemyWave wave in currentConfig.enemyWaves)
        {
            foreach (EnemyData data in wave.enemies)
            {
                GameObject prefab = data.enemyPrefab;
                int count = data.wavesEnemyCount;

                if (maxEnemies.ContainsKey(prefab))
                {
                    if (count > maxEnemies[prefab])
                        maxEnemies[prefab] = count;
                }
                else
                {
                    maxEnemies.Add(prefab, count);
                }
            }
        }

        // 预生成敌人
        foreach (var kvp in maxEnemies)
        {
            GameObject prefab = kvp.Key;
            int count = kvp.Value;

            if (!enemyPool.ContainsKey(prefab))
                enemyPool[prefab] = new Queue<Entity>();

            for (int i = 0; i < count; i++)
            {
                Entity enemy = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<Entity>();
                enemy.gameObject.SetActive(false);

                // 设置原始预制体引用
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent) enemyComponent.OriginalPrefab = prefab;

                enemyPool[prefab].Enqueue(enemy);
            }
        }
    }
    private void PreloadDestructibles()
    {
        if (currentConfig == null) return;

        Dictionary<GameObject, int> maxDestructibles = new Dictionary<GameObject, int>();

        // 计算每个敌人类型需要的最大数量
        foreach (EnemyWave des in currentConfig.enemyWaves)
        {
            foreach (DestructibleData data in des.destructibles)
            {
                GameObject prefab = data.destructiblePrefab;
                int count = data.wavesDestructiblesCount;

                if (maxDestructibles.ContainsKey(prefab))
                {
                    if (count > maxDestructibles[prefab])
                        maxDestructibles[prefab] = count;
                }
                else
                {
                    maxDestructibles.Add(prefab, count);
                }
            }
        }
        foreach (var kvp in maxDestructibles)
        {
            GameObject prefab = kvp.Key;
            int count = kvp.Value;

            if (!desPool.ContainsKey(prefab))
                desPool[prefab] = new Queue<Entity>();

            for (int i = 0; i < count; i++)
            {
                Entity des = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<Entity>();
                des.gameObject.SetActive(false);

                // 设置原始预制体引用
                Destructible enemyComponent = des.GetComponent<Destructible>();
                if (enemyComponent) enemyComponent.OriginalPrefab = prefab;

                desPool[prefab].Enqueue(des);
            }
        }
    }
    private void SpawnDes(GameObject prefab)
    {
        if (prefab == null || currentConfig == null) return;

        if (desPool.TryGetValue(prefab, out Queue<Entity> pool) && pool.Count > 0)
        {
            Entity des = pool.Dequeue();
            des.transform.position = GetDesSpawnPosition();
            des.gameObject.SetActive(true);
            Entity desComponent = des.GetComponent<Entity>();
            desComponent.OnDeath += () => HandleDesDestroy(desComponent);
            if (desComponent)
            {
                desComponent.ResetEntity();
            }
        }
    }

    private Vector3 GetDesSpawnPosition()
    {
        if (currentConfig == null || currentConfig.destructiblesSpawnPositions.Length == 0)
            return Vector3.zero;
        else
            return currentConfig.destructiblesSpawnPositions[Random.Range(0, currentConfig.destructiblesSpawnPositions.Length)];
    }

    public Vector3 GetPlayerSpawnPosition()
    {
        return currentConfig.playerSpawnPosition;
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || currentConfig == null) return;

        playerInstance = Instantiate(
            playerPrefab,
            GetPlayerSpawnPosition(),
            Quaternion.identity
        );
    }

    private IEnumerator StartNextWave()
    {
        if (currentConfig == null || currentConfig.enemyWaves.Count == 0) yield break;

        while (currentWaveIndex < currentConfig.enemyWaves.Count)
        {
            var currentWave = currentConfig.enemyWaves[currentWaveIndex];
            
            foreach (var enemyData in currentWave.enemies)
            {
                for (int i = 0; i < enemyData.wavesEnemyCount; i++)
                {
                    SpawnEnemy(enemyData.enemyPrefab);
                    yield return new WaitForSeconds(enemyData.spawnInterval);
                }
            }
            foreach (var desData in currentWave.destructibles)
            {
                for (int i = 0; i < desData.wavesDestructiblesCount; i++)
                {
                    SpawnDes(desData.destructiblePrefab);
                }
            }

            if (IsFinalWave())
                SpawnBoss();

            yield return new WaitUntil(() => activeEnemyCount <= 0);
            if (!IsFinalWave())
                SpawnPortal();
            portalTriggered = false;
            yield return new WaitUntil(() => portalTriggered);
            currentWaveIndex++;
        }
    }
    public void SetPortalTrigger()
    {
        if (portalTriggered) return;
        portalTriggered = true;
    }
    private void SpawnPortal()
    {
        GameObject portal = Instantiate(Portal);
        portal.transform.position = currentConfig.portalSpawnPositions[currentWaveIndex];
    }
    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || currentConfig == null) return;

        if (enemyPool.TryGetValue(prefab, out Queue<Entity> pool) && pool.Count > 0)
        {
            Entity enemy = pool.Dequeue();
            enemy.transform.position = GetRandomSpawnPosition();
            enemy.gameObject.SetActive(true);
            activeEnemyCount++;

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent)
            {
                enemyComponent.ResetEntity();
                enemyComponent.OnDeath += () => HandleEnemyDeath(enemyComponent);
            }
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        activeEnemyCount--;
        enemy.gameObject.SetActive(false);

        if (enemy.OriginalPrefab != null && enemyPool.ContainsKey(enemy.OriginalPrefab))
        {
            Entity entity = enemy.GetComponent<Entity>();
            enemyPool[enemy.OriginalPrefab].Enqueue(entity);
        }
    }
    private void HandleDesDestroy(Entity des)
    {
        des.gameObject.SetActive(false);

        if (des.OriginalPrefab != null && desPool.ContainsKey(des.OriginalPrefab))
        {
            Entity entity = des.GetComponent<Entity>();
            desPool[des.OriginalPrefab].Enqueue(entity);
        }
    }
    private void SpawnBoss()
    {
        if (currentConfig?.bossPrefab == null) return;

        var boss = Instantiate(
            currentConfig.bossPrefab,
            currentConfig.bossSpawnPosition,
            Quaternion.identity
        );
        activeEnemyCount++;
        Enemy bossEnemy = boss.GetComponent<Enemy>();
        if (bossEnemy)
        {
            bossEnemy.OnDeath += () => {
                activeEnemyCount--;
                Destroy(boss);
            };
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (currentConfig == null || currentConfig.enemySpawnPositions.Length == 0)
            return Vector3.zero;

        return currentConfig.enemySpawnPositions[
            Random.Range(0, currentConfig.enemySpawnPositions.Length)
        ];
    }

    private bool IsFinalWave()
    {
        return currentWaveIndex == currentConfig.enemyWaves.Count - 1;
    }
    public int ReturnCurrentWaveIndex() => currentWaveIndex;
}
[System.Serializable]
public class EnemyData
{
    public GameObject enemyPrefab;
    public float spawnInterval;
    public int wavesEnemyCount;
}
[System.Serializable]
public class DestructibleData
{
    public GameObject destructiblePrefab;
    public int wavesDestructiblesCount;
}
[System.Serializable]
public class EnemyWave
{
    public List<EnemyData> enemies;
    public List<DestructibleData> destructibles;
}