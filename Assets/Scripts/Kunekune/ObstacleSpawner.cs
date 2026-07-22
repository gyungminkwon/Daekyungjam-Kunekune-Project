using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject shortRicePrefab;
    public GameObject tallRicePrefab;
    public GameObject treePrefab;
    public GameObject endGoalPrefab;
    
    [Header("스폰 간격 (범위)")]
    public float minSpawnIntervalX = 0.3f;
    public float maxSpawnIntervalX = 0.5f;

    [Header("스폰 환경 설정")]
    public Transform player;
    public float startSpawnX = 64f;
    public float groundY = -24f;
    public int initialSpawnCount = 80;

    [Header("확률 및 패턴 설정")]
    [Range(0f, 1f)] 
    public float treeSpawnChance = 0.0025f;

    [Tooltip("벼가 한 번 나오면 연달아 나올 최소/최대 칸 수")]
    public int minRiceCluster = 1;
    public int maxRiceCluster = 5;

    [Tooltip("빈 공간이 한 번 나오면 연달아 비워둘 최소/최대 칸 수")]
    public int minEmptyCluster = 2;
    public int maxEmptyCluster = 4;

    private float nextSpawnX;
    private int currentPattern = -1;
    private int patternRemaining = 0;

    [Header("맵 종료 설정")]
    public int maxTotalSpawns = 200;
    
    private int currentSpawnCount = 0;
    private bool isFinished = false;

    void Start()
    {
        nextSpawnX = startSpawnX;

        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player != null && player.position.x + 15f > nextSpawnX)
        {
            SpawnSegment();
        }
    }

    private void SpawnSegment()
    {
        if (currentSpawnCount >= maxTotalSpawns)
        {
            if (!isFinished)
            {
                if (endGoalPrefab != null)
                {
                    Instantiate(endGoalPrefab, new Vector2(nextSpawnX, groundY), Quaternion.identity, transform);
                }
                
                isFinished = true;
                Debug.Log("맵 연장 끝");
            }
            return;
        }

        if (patternRemaining <= 0)
        {
            currentPattern = Random.Range(0, 3);

            if (currentPattern == 0) 
            {
                patternRemaining = Random.Range(minEmptyCluster, maxEmptyCluster + 1);
            }
            else 
            {
                patternRemaining = Random.Range(minRiceCluster, maxRiceCluster + 1);
            }
        }

        Vector2 spawnPos = new Vector2(nextSpawnX, groundY);

        if (currentPattern == 1)
        {
            Instantiate(shortRicePrefab, spawnPos, Quaternion.identity, transform);
        }
        else if (currentPattern == 2)
        {
            Instantiate(tallRicePrefab, spawnPos, Quaternion.identity, transform);
        }

        if (Random.value < treeSpawnChance)
        {
            Instantiate(treePrefab, spawnPos, Quaternion.identity, transform);
        }

        float interval = Random.Range(minSpawnIntervalX, maxSpawnIntervalX);
        nextSpawnX += interval;

        patternRemaining--;
        currentSpawnCount++;
    }
}