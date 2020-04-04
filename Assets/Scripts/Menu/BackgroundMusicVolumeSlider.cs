using UnityEngine;
using UnityEngine.UI;

/// <summary></summary>
public class BackgroundMusicVolumeSlider : MonoBehaviour
{
    void Start()
    {
        Slider slider = gameObject.GetComponent<Slider>();
        // Set value of slider to value of volume.
        slider.value = SettingsContainer.Instance.BackgroundMusicVolume;
        slider.onValueChanged.AddListener(value => SettingsContainer.Instance.BackgroundMusicVolume = value);
    }
}
