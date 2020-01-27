using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        SceneChanger.UnloadPauseMenuAsAdditiveScene();
    }

    public void OpenMainMenu()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        SceneChanger.SetMainMenuAsActiveScene();
    }
}
