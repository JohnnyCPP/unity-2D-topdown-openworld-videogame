using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Slider slider;
    public CanvasGroup CanvasGroup;

    void Start()
    {
        CanvasGroup.alpha = 0;
    }

    public void SetDuration (float duration)
    {
        CanvasGroup.alpha = 1;
        slider.maxValue = duration;
        slider.value = duration;
    }
    public void UpdateTime()
    {
        slider.value -= Time.deltaTime;
        if (slider.value <= 0)
            CanvasGroup.alpha = 0;
    }
}
