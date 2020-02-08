using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Image sliderFillImage;

    void Start()
    {
        // Lock and hide curser
        Cursor.lockState = CursorLockMode.Locked;
    }

    // A Slider in Unity can only set a highlight color when it is dragged or moved.
    // Since we only use the keyboard and gamepad for menu navigation we want it highlighted when it is selected.
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>() != null)
        {
            // Highlight the Slider of the menu when it is selected.
            if (sliderFillImage == null)
            {
                sliderFillImage = EventSystem.current.currentSelectedGameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
            }
            if (sliderFillImage.color.r != 0.8f)
            {
                Color color = sliderFillImage.color;
                color.r = 8f;
                sliderFillImage.color = color;
            }
            else if (sliderFillImage.color.r != 0.6f)
            {
                // Unhighlight the Slider of the menu when it is not selected.
                Color color = sliderFillImage.color;
                color.r = 0.6f;
                sliderFillImage.color = color;
            }
        }
    }
}
