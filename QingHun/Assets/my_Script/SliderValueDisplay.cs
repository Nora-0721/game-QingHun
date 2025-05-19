using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider targetSlider; // 拖拽你的Slider到这里
    public TMP_Text valueText;      // 拖拽你的Text组件到这里

    void Start()
    {
        // 初始值更新
        UpdateTextValue(targetSlider.value);

        // 添加监听器：当Slider值变化时自动调用
        targetSlider.onValueChanged.AddListener(UpdateTextValue);
    }

    // 值更新方法
    private void UpdateTextValue(float value)
    {
        if (targetSlider.name == "BossHealthBar")
        {
            value *= 15;
        }
        if (targetSlider.name == "PlayerHealthBar")
        {
            value *= 50;
        }
        if (targetSlider.name == "BossHealthBarbossz")
        {
            value *= 100;
        }
        if (targetSlider.name == "PlayerHealthBarbossz")
        {
            value *= 9999;
        }
        if (targetSlider.name == "BossHealthBarboss")
        {
            value *= 100;
        }
        if (targetSlider.name == "PlayerHealthBarboss")
        {
            value *= 15;
        }
        if (targetSlider.name == "BossHealthBar333")
        {
            value *= 40;
        }
        if (targetSlider.name == "PlayerHealthBar333")
        {
            value *= 200;
        }
        valueText.text = value.ToString("0"); // 格式化为两位小数
    }
}