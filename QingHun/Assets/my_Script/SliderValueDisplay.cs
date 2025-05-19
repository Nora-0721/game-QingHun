using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider targetSlider; // ��ק���Slider������
    public TMP_Text valueText;      // ��ק���Text���������

    void Start()
    {
        // ��ʼֵ����
        UpdateTextValue(targetSlider.value);

        // ��Ӽ���������Sliderֵ�仯ʱ�Զ�����
        targetSlider.onValueChanged.AddListener(UpdateTextValue);
    }

    // ֵ���·���
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
        valueText.text = value.ToString("0"); // ��ʽ��Ϊ��λС��
    }
}