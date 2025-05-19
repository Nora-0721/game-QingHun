// AutoMover.cs
using UnityEngine;

public class AutoMover : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }
}