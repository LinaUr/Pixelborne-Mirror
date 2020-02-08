using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Image m_sliderFillImage;

    private const float m_HIGHLIGHT = 0.8f;
    private const float m_UNHIGHLIGHT = 0.6f;

    void Start()
    {
        // Lock and hide curser.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // A Slider in Unity can only set a highlight color when it is dragged or moved.
    // Since we only use the keyboard and gamepad for menu navigation we want it highlighted when it is selected.
    private void Update()
    {
        // Check if a Slider is selected.
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>() != null)
        {
            // Not all Menus have a Slider, but when it does we grab it.
            if (m_sliderFillImage == null)
            {
                m_sliderFillImage = EventSystem.current.currentSelectedGameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
            }
            // Highlight the Slider of the menu when it is selected.
            if (m_sliderFillImage.color.r != m_HIGHLIGHT)
            {
                Color color = m_sliderFillImage.color;
                color.r = m_HIGHLIGHT;
                m_sliderFillImage.color = color;
            }
            
        }
        else if (m_sliderFillImage != null && m_sliderFillImage.color.r != m_UNHIGHLIGHT)
        {
            // Unhighlight the Slider of the menu when it is not selected.
            Color color = m_sliderFillImage.color;
            color.r = m_UNHIGHLIGHT;
            m_sliderFillImage.color = color;
        }
    }
}
