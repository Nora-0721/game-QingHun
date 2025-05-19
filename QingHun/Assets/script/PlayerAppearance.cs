using UnityEngine;
using System.Collections;
public class PlayerAppearance : MonoBehaviour
{
    [Header("角色外观")]
    [SerializeField] private Sprite normalSprite;   // 正常状态的图案
    [SerializeField] private Sprite damagedSprite;  // 受伤状态的图案

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("找不到 SpriteRenderer 组件！");
        }
        else
        {
            spriteRenderer.sprite = normalSprite; // 初始化为正常状态
        }
    }

    // 外部调用的受伤效果方法
    public void TriggerDamageEffect()
    {
        if (spriteRenderer != null && damagedSprite != null)
        {
            spriteRenderer.sprite = damagedSprite;
            StartCoroutine(RecoverSprite());
        }
    }

    private IEnumerator RecoverSprite()
    {
        yield return new WaitForSeconds(0.5f); // 0.5秒后恢复
        spriteRenderer.sprite = normalSprite;
        Debug.Log("角色外观恢复为正常状态");
    }
}