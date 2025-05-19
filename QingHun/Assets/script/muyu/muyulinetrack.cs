// muyulinetrack.cs
using UnityEngine;
using System.Collections.Generic;

public class muyulinetrack : MonoBehaviour
{
    public Sprite originalSprite;
    public Sprite replacementSprite;
    public Sprite thirdSprite;

    private SpriteRenderer spriteRenderer;
    private HashSet<int> scoredPoints = new HashSet<int>();
    private KeyCode activationKey;
    private float spriteChangeTimer;
    private Collider2D currentCollision;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = originalSprite;
        SetActivationKey();
    }

    private void SetActivationKey()
    {
        float xPos = transform.position.x;
        activationKey = xPos switch
        {
            _ when Mathf.Approximately(xPos, 0f) => KeyCode.J,
            _ when Mathf.Approximately(xPos, -2f) => KeyCode.D,
            _ when Mathf.Approximately(xPos, -4f) => KeyCode.S,
            _ when Mathf.Approximately(xPos, 2f) => KeyCode.K,
            _ when Mathf.Approximately(xPos, 4f) => KeyCode.L,
            _ => KeyCode.None
        };
    }

    private void Update()
    {
        HandleSpriteReset();
        HandleInteraction();
    }

    private void HandleSpriteReset()
    {
        if (spriteRenderer.sprite == originalSprite) return;

        spriteChangeTimer -= Time.deltaTime;
        if (spriteChangeTimer <= 0)
            spriteRenderer.sprite = originalSprite;
    }

    private void HandleInteraction()
    {
        if (!Input.GetKey(activationKey) || currentCollision == null) return;

        Vector2 selfCenter = GetComponent<Collider2D>().bounds.center;
        Vector2 otherCenter = currentCollision.bounds.center;
        float offset = Vector2.Distance(selfCenter, otherCenter);

        UpdateSprite(offset);
        TryAddScore(offset);
    }

    private void UpdateSprite(float offset)
    {
        spriteChangeTimer = 0.8f;
        spriteRenderer.sprite = offset switch
        {
            < 0.2f => replacementSprite,
            < 0.5f => thirdSprite,
            _ => originalSprite
        };
    }

    private void TryAddScore(float offset)
    {
        int pointID = currentCollision.GetInstanceID();
        if (scoredPoints.Contains(pointID)) return;

        switch (offset)
        {
            case < 0.2f:
                ScoreManager.Instance.AddScore(30);
                scoredPoints.Add(pointID);
                break;
            case < 0.5f:
                ScoreManager.Instance.AddScore(10);
                scoredPoints.Add(pointID);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("point")) return;

        currentCollision = other;
        spriteChangeTimer = 0.8f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("point") && currentCollision == other)
            currentCollision = null;
    }
}