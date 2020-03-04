using UnityEngine;

public class SettingsContainer : ScriptableObject
{
    public float BackgroundMusicVolume { get; set; } = 1;
    private static SettingsContainer m_instance = null;
    

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
