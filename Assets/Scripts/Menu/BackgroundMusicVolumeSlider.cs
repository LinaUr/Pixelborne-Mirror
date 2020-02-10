using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicVolumeSlider : MonoBehaviour
{
    void Start()
    {
        // Set value of slider to value of volume.
        gameObject.GetComponent<Slider>().value = GameMediator.Instance.BackgroundMusicVolume;
    }

    void Update()
    {
        // Set value of volume to value of slider.
        GameMediator.Instance.BackgroundMusicVolume = gameObject.GetComponent<Slider>().value; 
    }
}
