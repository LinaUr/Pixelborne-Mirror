using UnityEngine.SceneManagement;

public class SceneChanger
{
    // MAIN_MENU_SCENE_INDEX represents the build index of the MainMenu scene.
    // The index must be taken from the build settings.
    static readonly int MAIN_MENU_SCENE_INDEX = 0;

    // SINGLEPLAYER_SCENE_INDEX represents the build index of the Singleplayer scene.
    // The index must be taken from the build settings.
    static readonly int SINGLEPLAYER_SCENE_INDEX = 1;
    
    // MULTIPLAYER_SCENE_INDEX represents the build index of the Multiplayer scene.
    // The index must be taken from the build settings.
    static readonly int MULTIPLAYER_SCENE_INDEX = 2;

    // This method sets the MainMenu scene as the active scene.
    public static void SetMainMenuAsActiveScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
    }
    
    // This method sets the Singleplayer scene as the active scene.
    public static void SetSingleplayerAsActiveScene()
    {
        SceneManager.LoadScene(SINGLEPLAYER_SCENE_INDEX);
    }

    // This method sets the Multiplayer scene as the active scene.
    public static void SetMultiplayerAsActiveScene()
    {
        SceneManager.LoadScene(MULTIPLAYER_SCENE_INDEX);
    }
}
