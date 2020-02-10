﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderHighlightColor : MonoBehaviour
{
    [SerializeField]
    private Image m_sliderFillImage;

    private const float m_HIGHLIGHT = 0.8f;
    private const float m_UNHIGHLIGHT = 0.6f;

    // A Slider in Unity can only set a highlight color when it is dragged or moved.
    // Since we only use the keyboard and gamepad for menu navigation we want it highlighted when it is selected.
    private void Update()
    {
        // Check if this Slider is selected.
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            // Highlight the Slider when it is selected.
            if (m_sliderFillImage.color.r != m_HIGHLIGHT)
            {
                Color color = m_sliderFillImage.color;
                color.r = m_HIGHLIGHT;
                m_sliderFillImage.color = color;
            }
            
        }
        else if (m_sliderFillImage.color.r != m_UNHIGHLIGHT)
        {
            // Unhighlight the Slider when it is not selected.
            Color color = m_sliderFillImage.color;
            color.r = m_UNHIGHLIGHT;
            m_sliderFillImage.color = color;
        }
    }
}