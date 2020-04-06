using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static AudioSource s_player;

    public static void SetVolume(float value)
    {
        if(s_player != null)
        {
            s_player.volume = value;
        }
    }

    void Start()
    {
        s_player = gameObject.GetComponent<AudioSource>();
        s_player.volume = SettingsContainer.Instance.BackgroundMusicVolume;
    }
}
