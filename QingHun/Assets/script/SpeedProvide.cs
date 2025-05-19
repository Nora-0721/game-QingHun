// SpeedProvider.cs（完整修正版）
using UnityEngine;

public class SpeedProvider : MonoBehaviour
{
    [SerializeField] private float _currentSpeed = 12f;

    // 修正参数类型为float
    public void SyncSpeed(float newSpeed)
    {
        _currentSpeed = newSpeed;
    }

    public float GetCurrentSpeed()
    {
        return _currentSpeed;
    }
}