using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

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
