// HealthRewardSpawner.cs ����������
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HealthRewardSpawner : MonoBehaviour
{
    [Header("��������")]
    public GameObject rewardPrefab;
    public float[] lanesY = { 3.45f, 0.36f, -2.66f };
    public float spawnX = 8f; // ȷ������Ļ��
    public float baseSafeOffset = 4f; // ��ȫ����

    private ObstacleSpawner _obstacleSpawner;
    private Camera _mainCamera;
    private float _rightScreenEdge;

    void Start()
    {
        _mainCamera = Camera.main;
        _obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
        if (_obstacleSpawner == null) Debug.LogError("δ�ҵ��ϰ�����������");
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        Debug.Log("�����������Ѽ���");
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
            Debug.LogWarning("��ǰ�ް�ȫ�������");
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
        Debug.Log($"���{trackIndex}��ȫ��{isSafe} | �ϰ����Ե��{obstacleRight} | �����Ե��{requireRight}");

        return isSafe;
    }

    void GenerateReward(int trackIndex)
    {
        // ��̬��������λ��
        float cameraRightEdge = _mainCamera.ViewportToWorldPoint(Vector3.right).x;
        Vector2 spawnPos = new Vector2(
            cameraRightEdge + 2f, // ����Ļ�Ҳ���2��λ����
            lanesY[trackIndex]
        );

        GameObject reward = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
        reward.GetComponent<HealthReward>().Initialize(_obstacleSpawner.baseSpeed);
        Debug.Log($"�������ɳɹ� | ��Ļ�ұ�Ե:{cameraRightEdge} | ����:{spawnPos}");
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