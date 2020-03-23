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
            return m_instance == null ? CreateInstance<SettingsContainer>() : m_instance;
        }
        private set { }
    }

    public SettingsContainer()
    {
        m_instance = this;
    }
}
