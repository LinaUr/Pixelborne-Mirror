using UnityEngine.SceneManagement;

public class SceneChanger
{
    // The following indices represent the build index of the corresponding scene.
    // The index must be taken from the build settings.
    static readonly int MAIN_MENU_SCENE_INDEX = 0;
    static readonly int PAUSE_MENU_SCENE_INDEX = 1;
    static readonly int MULTIPLAYER_SCENE_INDEX = 2;
    static readonly int SINGLEPLAYER_SCENE_INDEX = 3;
    static readonly int SELLING_SCREEN_SCENE_INDEX = 4;
    static readonly int WINNING_SCREEN_SCENE_INDEX = 5;
    static readonly int[] SINGLEPLAYER_STAGES_INDICES = { 6, 7, 8 };

    public static void SetMainMenuAsActiveScene()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_INDEX);
    }

    public static void SetSingleplayerAsActiveScene()
    {
        SceneManager.LoadScene(SINGLEPLAYER_SCENE_INDEX);
    }

    // This method returns a bool to indicate whether the requested stage exists and could be loaded.
    public static bool LoadSingleplayerStageAsActiveScene(int index)
    {
        if (index > SINGLEPLAYER_STAGES_INDICES.Length - 1)
        {
            return false;
        }
        SceneManager.LoadScene(SINGLEPLAYER_STAGES_INDICES[index]);
        return true;
    }

    public static void SetMultiplayerAsActiveScene()
    {
        SceneManager.LoadScene(MULTIPLAYER_SCENE_INDEX);
    }
    
    public static void SetWinningScreenAsActiveScene()
    {
        SceneManager.LoadScene(WINNING_SCREEN_SCENE_INDEX);
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
}
