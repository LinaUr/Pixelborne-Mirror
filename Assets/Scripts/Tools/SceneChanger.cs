using UnityEngine.SceneManagement;

public class SceneChanger
{
    // MAIN_MENU_SCENE_INDEX represents the build index of the MainMenu scene.
    // The index must be taken from the build settings.
    static readonly int MAIN_MENU_SCENE_INDEX = 0;

    // PAUSE_MENU_SCENE_INDEX represents the build index of the MainMenu scene.
    // The index must be taken from the build settings.
    static readonly int PAUSE_MENU_SCENE_INDEX = 1;

    // MULTIPLAYER_SCENE_INDEX represents the build index of the Multiplayer scene.
    // The index must be taken from the build settings.
    static readonly int MULTIPLAYER_SCENE_INDEX = 2;

    // TODO: uncomment when Singleplayer Scene gets introduced.
    // SINGLEPLAYER_SCENE_INDEX represents the build index of the Singleplayer scene.
    // The index must be taken from the build settings.
    //static readonly int SINGLEPLAYER_SCENE_INDEX = 3;

    // This method sets the Multiplayer scene as the active scene.
    public static void SetMultiplayerAsActiveScene()
    {
        SceneManager.LoadScene(MULTIPLAYER_SCENE_INDEX);
    }

    // This method sets the MainMenu scene as the active scene.
    public static void SetMainMenuAsActiveScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
    }

    // This method sets the PauseMenu scene as the active scene.
    public static void LoadPauseMenuAsAdditiveScene()
    {
        bool sceneAlreadyLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).buildIndex == PAUSE_MENU_SCENE_INDEX)
            {
                sceneAlreadyLoaded = true;
            }
        }

        if (!sceneAlreadyLoaded)
        {
            SceneManager.LoadScene(PAUSE_MENU_SCENE_INDEX, LoadSceneMode.Additive);
        }
    }
    public static void UnloadPauseMenuAsAdditiveScene()
    {
        SceneManager.UnloadSceneAsync(PAUSE_MENU_SCENE_INDEX);
    }

    // TODO: uncomment when Singleplayer Scene gets introduced.
    // This method sets the MainMenu scene as the active scene.
    //public static void SetSingleplayerAsActiveScene()
    //{
    //    SceneManager.LoadScene(SINGLEPLAYER_SCENE_INDEX);
    //}
}
