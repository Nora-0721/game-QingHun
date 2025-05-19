// HealthRewardSpawner.cs 完整修正版
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HealthRewardSpawner : MonoBehaviour
{
    [Header("基础设置")]
    public GameObject rewardPrefab;
    public float[] lanesY = { 3.45f, 0.36f, -2.66f };
    public float spawnX = 8f; // 确保在屏幕内
    public float baseSafeOffset = 4f; // 安全距离

    private ObstacleSpawner _obstacleSpawner;
    private Camera _mainCamera;
    private float _rightScreenEdge;

    void Start()
    {
        _mainCamera = Camera.main;
        _obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
        if (_obstacleSpawner == null) Debug.LogError("未找到障碍物生成器！");
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        Debug.Log("奖励生成器已激活");
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 6f));
            TrySpawnReward();
        }
    }

    void TrySpawnReward()
    {
        UpdateScreenEdges();
        var safeTracks = GetSafeTracks().ToList();

        if (safeTracks.Count == 0)
        {
            Debug.LogWarning("当前无安全轨道可用");
            return;
        }

        int track = safeTracks[Random.Range(0, safeTracks.Count)];
        GenerateReward(track);
    }

    List<int> GetSafeTracks()
    {
        return lanesY.Select((_, i) => i).Where(IsTrackSafe).ToList();
    }

    bool IsTrackSafe(int trackIndex)
    {
        var lastObstacle = _obstacleSpawner.GetActiveObstaclesOnLane(trackIndex)
                          .OrderByDescending(o => o.transform.position.x)
                          .FirstOrDefault();

        if (lastObstacle == null) return true;

        ObstacleController ctrl = lastObstacle.GetComponent<ObstacleController>();
        float obstacleRight = ctrl.RightEdge;
        float requireRight = _rightScreenEdge - (baseSafeOffset + ctrl.Width);

        bool isSafe = obstacleRight < requireRight;
        Debug.Log($"轨道{trackIndex}安全：{isSafe} | 障碍物边缘：{obstacleRight} | 需求边缘：{requireRight}");

        return isSafe;
    }

    void GenerateReward(int trackIndex)
    {
        // 动态计算生成位置
        float cameraRightEdge = _mainCamera.ViewportToWorldPoint(Vector3.right).x;
        Vector2 spawnPos = new Vector2(
            cameraRightEdge + 2f, // 在屏幕右侧外2单位生成
            lanesY[trackIndex]
        );

        GameObject reward = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
        reward.GetComponent<HealthReward>().Initialize(_obstacleSpawner.baseSpeed);
        Debug.Log($"奖励生成成功 | 屏幕右边缘:{cameraRightEdge} | 坐标:{spawnPos}");
    }


    void UpdateScreenEdges()
    {
        _rightScreenEdge = _mainCamera.ViewportToWorldPoint(Vector3.right).x;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (float y in lanesY)
        {
            Gizmos.DrawWireCube(new Vector3(spawnX, y, 0), new Vector3(1, 1, 0));
        }
    }
}