using UnityEngine;

public class BackgroundMusicVolume : MonoBehaviour
{
    void Update()
    {
        // Set value of AudioSource volume.
        gameObject.GetComponent<AudioSource>().volume = SettingsContainer.Instance.BackgroundMusicVolume;
    }
}
