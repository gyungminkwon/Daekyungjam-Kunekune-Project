using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject shortRicePrefab;
    public GameObject tallRicePrefab;
    public GameObject endGoalPrefab;
    public GameObject frontPillarPrefab;
    
    [Header("스폰 간격 (범위)")]
    public float minSpawnIntervalX = 0.3f;
    public float maxSpawnIntervalX = 0.5f;

    [Header("스폰 환경 설정")]
    public Transform player;
    public float startSpawnX = 643;
    public float groundY = -24.07f;
    public int initialSpawnCount = 20;

    [Header("확률 및 패턴 설정")]
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
    public int maxTotalSpawns = 120;
    
    [Tooltip("엔드 골이 스폰되기 전 빈 공간 거리")]
    public float emptySpaceBeforeGoal = 4f;

    [Tooltip("종료 지점 프리팹 Y 좌표")]
    public float endGoalY = -24.97f;

    [Header("신사 앞기둥 좌표 설정")]
    [Tooltip("엔드 골 X 좌표 기준 오프셋")]
    public float frontPillarOffsetX = 0f;
    [Tooltip("신사 앞기둥 프리팹 Y 좌표")]
    public float frontPillarY = -24.97f;

    [Header("엔딩 타임라인")]
    [Tooltip("앞기둥 기준 X 좌표 오프셋")]
    public float endingTriggerOffset = 1.5f;
    // public PlayableDirector Timeline;

    private int currentSpawnCount = 0;
    private bool isFinished = false;
    private float frontPillarSpawnX = 0f; 
    private bool isEndingTriggered = false;
    
    private GameObject spawnedFrontPillar = null;
    private GameObject spawnedEndGoal = null; 

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
        if (player == null) return;

        if (!isFinished && player.position.x + 15f > nextSpawnX)
        {
            SpawnSegment();
        }

        if (isFinished && !isEndingTriggered)
        {
            if (player.position.x >= frontPillarSpawnX + endingTriggerOffset)
            {
                isEndingTriggered = true;

                if (spawnedFrontPillar != null)
                {
                    spawnedFrontPillar.SetActive(false);
                }
                
                if (spawnedEndGoal != null)
                {
                    Collider2D goalCollider = spawnedEndGoal.GetComponent<Collider2D>();
                    if (goalCollider != null)
                    {
                        goalCollider.isTrigger = false;
                    }
                }
                
                // StartCoroutine(PlayEndingSequence());
            }
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
                    float finalGoalX = nextSpawnX + emptySpaceBeforeGoal;
                    
                    spawnedEndGoal = Instantiate(endGoalPrefab, new Vector2(finalGoalX, endGoalY), Quaternion.identity, transform);

                    if (frontPillarPrefab != null)
                    {
                        frontPillarSpawnX = finalGoalX + frontPillarOffsetX;
                        spawnedFrontPillar = Instantiate(frontPillarPrefab, new Vector2(frontPillarSpawnX, frontPillarY), Quaternion.identity, transform);
                    }
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

        float interval = Random.Range(minSpawnIntervalX, maxSpawnIntervalX);
        nextSpawnX += interval;

        patternRemaining--;
        currentSpawnCount++;
    }

    // IEnumerator PlayEndingSequence()
    // {
    //     Debug.Log("엔딩 타임라인");

    //     float fadeTime = 2f;

    //     UIManager.Instance.FadeUI(0f, 1f, fadeTime);
    //     yield return new WaitForSeconds(fadeTime);

    //     if (timelineDirector != null)
    //     {
    //         timelineDirector.Play();
    //     }
    // }
}