using UnityEngine;

public class SettingsContainer : ScriptableObject
{
    private static SettingsContainer m_instance = null;
    private float m_backgroundMusicVolume = 1.0f;

    public float BackgroundMusicVolume
    {
        get
        {
            return m_backgroundMusicVolume;
        }
        set
        {
            m_backgroundMusicVolume = value;
            BackgroundMusic.SetVolume(value);
        }
    }

    public static SettingsContainer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return s_instance == null ? CreateInstance<SettingsContainer>() : s_instance;
        }
        private set { }
    }

    public SettingsContainer()
    {
        s_instance = this;
    }
}
