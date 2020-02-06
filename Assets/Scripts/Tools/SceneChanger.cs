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

    // SINGLEPLAYER_SCENE_INDEX represents the build index of the Singleplayer scene.
    // The index must be taken from the build settings.
    static readonly int SINGLEPLAYER_SCENE_INDEX = 3;

    // SELLING_SCREEN_SCENE_INDEX represents the build index of the SellingScreen scene.
    // The index must be taken from the build settings.
    static readonly int SELLING_SCREEN_SCENE_INDEX = 4;

    // WINNING_SCREEN_SCENE_INDEX represents the build index of the WinningScreen scene.
    // The index must be taken from the build settings.
    static readonly int WINNING_SCREEN_SCENE_INDEX = 5;

    public static void SetMainMenuAsActiveScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
    }

    public static void SetSingleplayerAsActiveScene()
    {
        SceneManager.LoadScene(SINGLEPLAYER_SCENE_INDEX);
    }

    public static void SetMultiplayerAsActiveScene()
    {
        SceneManager.LoadScene(MULTIPLAYER_SCENE_INDEX);
    }

    // This method loads a scene with the index additive to the current scene.
    public static void LoadSceneAdditive(int index)
    {
        // Check if the scene has already been loaded additive.
        bool sceneAlreadyLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).buildIndex == index)
            {
                sceneAlreadyLoaded = true;
            }
        }

        if (!sceneAlreadyLoaded)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Additive);
        }
    }

    // This method loads PauseMenu additive to the scene.
    public static void LoadPauseMenuAdditive()
    {
        LoadSceneAdditive(PAUSE_MENU_SCENE_INDEX);
    }

    // This method removes the PauseMenu as additive scene.
    public static void UnloadPauseMenuAdditive()
    {
        SceneManager.UnloadSceneAsync(PAUSE_MENU_SCENE_INDEX);
    }

    // This method loads SellingScreen additive to the scene.
    public static void LoadSellingScreenAdditive()
    {
        LoadSceneAdditive(SELLING_SCREEN_SCENE_INDEX);
    }

    // This method removes the SellingScreen as additive scene.
    public static void UnloadSellingScreenAdditive()
    {
        SceneManager.UnloadSceneAsync(SELLING_SCREEN_SCENE_INDEX);
    }

    // This method loads WinningScreen additive to the scene.
    public static void LoadWinningScreenAdditive()
    {
        LoadSceneAdditive(WINNING_SCREEN_SCENE_INDEX);
    }

    // This method removes the WinningScreen as additive scene.
    public static void UnloadWinningScreenAdditive()
    {
        SceneManager.UnloadSceneAsync(WINNING_SCREEN_SCENE_INDEX);
    }
}
