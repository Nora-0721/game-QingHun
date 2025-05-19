using UnityEngine;
using System.Collections;
public class PlayerAppearance : MonoBehaviour
{
    [Header("��ɫ���")]
    [SerializeField] private Sprite normalSprite;   // ����״̬��ͼ��
    [SerializeField] private Sprite damagedSprite;  // ����״̬��ͼ��

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("�Ҳ��� SpriteRenderer �����");
        }
        else
        {
            spriteRenderer.sprite = normalSprite; // ��ʼ��Ϊ����״̬
        }
    }

    // �ⲿ���õ�����Ч������
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
        yield return new WaitForSeconds(0.5f); // 0.5���ָ�
        spriteRenderer.sprite = normalSprite;
        Debug.Log("��ɫ��ۻָ�Ϊ����״̬");
    }
}