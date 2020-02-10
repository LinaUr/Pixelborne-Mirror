using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        // Freeze game.
        Time.timeScale = 0;
        GameMediator.Instance.LockPlayerInput(true);

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void Resume()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        SceneChanger.UnloadPauseMenuAdditive();
        GameMediator.Instance.LockPlayerInput(false);
    }

    public void OpenMainMenu()
    {
        // Unfreeze game.
        Time.timeScale = 1;
        GameMediator.Instance.LockPlayerInput(false);
        SceneChanger.SetMainMenuAsActiveScene();
    }
}
