using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FlipSprite : MonoBehaviour
{
    public float flipInterval; // 翻转间隔（秒）
    private SpriteRenderer spriteRenderer;
    private int flipCount = 0; // 新增翻转计数器

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        while (true)
        {
            // 切换方向
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // 等待指定时间
            yield return new WaitForSeconds(flipInterval);

            // 增加翻转计数
            flipCount++;

            // 如果翻转次数达到两次，退出协程
            if (flipCount >= 4)
            {
                SceneManager.LoadScene("dialog2");
                yield break;
            }
        }
    }

    public class SceneSwitcher : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}