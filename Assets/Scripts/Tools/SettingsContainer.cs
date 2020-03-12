using UnityEngine;

public class SettingsContainer : ScriptableObject
{
    private static SettingsContainer s_instance = null;

    public float BackgroundMusicVolume { get; set; } = 1;
    
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
