// SpeedProvider.cs�����������棩
using UnityEngine;

public class SpeedProvider : MonoBehaviour
{
    [SerializeField] private float _currentSpeed = 12f;

    // ������������Ϊfloat
    public void SyncSpeed(float newSpeed)
    {
        _currentSpeed = newSpeed;
    }

    public float GetCurrentSpeed()
    {
        return _currentSpeed;
    }
}