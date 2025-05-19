// ObstacleSpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("基础设置")]
    public GameObject obstaclePrefab;
    public float[] lanesY = { 3.45f, 0.36f, -2.66f };
    public float baseSpeed = 12f;

    [Header("生成控制")]
    [SerializeField] private Vector2 _lengthRange = new Vector2(0.8f, 1.5f);
    [SerializeField] private float _minSpawnInterval = 1.0f;
    [SerializeField] private float _maxSpawnInterval = 1.5f;

    [Header("随机化系统")]
    [SerializeField] private float _randomBias = 0.4f;    // 随机偏移量（0-1）
    [SerializeField] private float _weightPower = 2.5f;   // 权重指数
    [SerializeField] private int _historySize = 7;        // 历史记录长度

    [Header("安全系统")]
    [SerializeField] private float _baseSafeOffset = 3f;
    [SerializeField] private float _speedFactor = 0.2f;

    private Camera _mainCamera;
    private Queue<GameObject> _obstaclePool = new Queue<GameObject>();
    private List<GameObject> _activeObstacles = new List<GameObject>();
    private List<int> _spawnHistory = new List<int>();
    private float _leftScreenEdge;
    private float _rightScreenEdge;
    private float _currentSpeed;

    void Start()
    {
        _mainCamera = Camera.main;
        _currentSpeed = baseSpeed;
        InitializePool(35);
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            UpdateSystemState();

            if (ShouldSpawnNewObstacle())
            {
                TrySpawnObstacle();
            }

            yield return new WaitForSeconds(GetSpawnInterval());
        }
    }

    void TrySpawnObstacle()
    {
        var availableTracks = GetAvailableTracks().ToList();
        if (availableTracks.Count == 0) return;

        int selectedTrack = ChooseTrackWithSmartRandom(availableTracks);
        SpawnObstacle(selectedTrack);
        UpdateSpawnHistory(selectedTrack);
    }

    List<int> GetAvailableTracks()
    {
        return lanesY
            .Select((_, index) => index)
            .Where(IsTrackSafe)
            .ToList();
    }

    bool IsTrackSafe(int trackIndex)
    {
        var lastObstacle = _activeObstacles
            .Where(o => o.activeSelf)
            .Select(o => o.GetComponent<ObstacleController>())
            .Where(c => c.TrackIndex == trackIndex)
            .OrderByDescending(c => c.RightEdge)
            .FirstOrDefault();

        if (lastObstacle == null) return true;

        float speedCompensation = _currentSpeed * _speedFactor;
        float minDistance = _baseSafeOffset + speedCompensation + lastObstacle.Width;
        return lastObstacle.RightEdge < _rightScreenEdge - minDistance;
    }

    int ChooseTrackWithSmartRandom(List<int> availableTracks)
    {
        // 计算每个轨道的权重
        var weights = availableTracks.ToDictionary(
            t => t,
            t => Mathf.Pow(1f / (GetRecentSpawnCount(t) + 1), _weightPower)
        );

        // 应用随机偏移
        float totalWeight = weights.Values.Sum() + _randomBias;
        float randomPoint = Random.Range(0f, totalWeight);

        // 概率选择（带洗牌防止顺序偏差）
        foreach (var track in availableTracks.OrderBy(_ => Random.value))
        {
            float trackWeight = weights[track] + _randomBias / availableTracks.Count;
            if (randomPoint < trackWeight) return track;
            randomPoint -= trackWeight;
        }

        return availableTracks[Random.Range(0, availableTracks.Count)];
    }

    int GetRecentSpawnCount(int trackIndex)
    {
        return _spawnHistory.Count(s => s == trackIndex);
    }

    void UpdateSpawnHistory(int newTrack)
    {
        _spawnHistory.Add(newTrack);
        if (_spawnHistory.Count > _historySize)
        {
            _spawnHistory.RemoveAt(0);
        }
    }

    void SpawnObstacle(int trackIndex)
    {
        GameObject obstacle = GetPooledObstacle();
        SetupObstacle(obstacle, trackIndex);
        _activeObstacles.Add(obstacle);
    }

    void SetupObstacle(GameObject obj, int trackIndex)
    {
        float length = Random.Range(_lengthRange.x, _lengthRange.y);
        Vector3 spawnPos = CalculateSpawnPosition(trackIndex, length);

        obj.transform.position = spawnPos;
        obj.SetActive(true);

        ObstacleController controller = obj.GetComponent<ObstacleController>();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

        sr.size = new Vector2(length, sr.size.y);
        controller.Initialize(trackIndex, _currentSpeed);
        controller.UpdateSize();
    }

    Vector3 CalculateSpawnPosition(int trackIndex, float length)
    {
        float safeOffset = CalculateSafeOffset(trackIndex, length);
        return new Vector3(
            _rightScreenEdge + length * 0.5f + safeOffset,
            lanesY[trackIndex],
            0
        );
    }

    float CalculateSafeOffset(int trackIndex, float newLength)
    {
        var lastObstacle = _activeObstacles
            .Where(o => o.activeSelf)
            .Select(o => o.GetComponent<ObstacleController>())
            .Where(c => c.TrackIndex == trackIndex)
            .OrderByDescending(c => c.RightEdge)
            .FirstOrDefault();

        if (lastObstacle == null) return _baseSafeOffset;

        float speedCompensation = _currentSpeed * _speedFactor;
        float minSafeDistance = _baseSafeOffset + speedCompensation + (lastObstacle.Width + newLength) * 0.6f;
        float existingSpace = _rightScreenEdge - lastObstacle.RightEdge;

        return Mathf.Max(minSafeDistance - existingSpace, _baseSafeOffset);
    }

    #region Helper Methods
    void UpdateSystemState()
    {
        UpdateScreenEdges();
        CleanupObstacles();
    }

    void UpdateScreenEdges()
    {
        _leftScreenEdge = _mainCamera.ViewportToWorldPoint(Vector3.zero).x;
        _rightScreenEdge = _mainCamera.ViewportToWorldPoint(Vector3.right).x;
    }

    bool ShouldSpawnNewObstacle()
    {
        return _activeObstacles.Count < _obstaclePool.Count * 0.7f;
    }

    float GetSpawnInterval()
    {
        return Random.Range(_minSpawnInterval, _maxSpawnInterval);
    }
    #endregion

    #region Pool Management
    void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab);
            obj.SetActive(false);
            _obstaclePool.Enqueue(obj);
        }
    }

    GameObject GetPooledObstacle()
    {
        if (_obstaclePool.Count < 5) ExpandPool(10);
        return _obstaclePool.Dequeue();
    }

    void ExpandPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab);
            obj.SetActive(false);
            _obstaclePool.Enqueue(obj);
        }
    }

    void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        _obstaclePool.Enqueue(obj);
    }
    #endregion

    #region Cleanup
    void CleanupObstacles()
    {
        float destroyThreshold = _leftScreenEdge - 8f;

        foreach (var obstacle in _activeObstacles.ToList())
        {
            if (!obstacle.activeSelf) continue;

            var controller = obstacle.GetComponent<ObstacleController>();
            if (controller.RightEdge < destroyThreshold)
            {
                ReturnToPool(obstacle);
                _activeObstacles.Remove(obstacle);
            }
        }
    }

    void OnDestroy()
    {
        foreach (var obj in _obstaclePool) Destroy(obj);
    }
    #endregion

    #region Debug
    
    #endregion
    public IEnumerable<GameObject> GetActiveObstaclesOnLane(int laneIndex)
    {
        return _activeObstacles
            .Where(o => o.activeSelf)
            .Select(o => o.GetComponent<ObstacleController>())
            .Where(c => c.TrackIndex == laneIndex)
            .Select(c => c.gameObject);
    }
    // 添加获取所有活动障碍物的方法
    public List<GameObject> GetActiveObstacles()
    {
        return _activeObstacles
            .Where(o => o.activeSelf)
            .ToList();
    }
}