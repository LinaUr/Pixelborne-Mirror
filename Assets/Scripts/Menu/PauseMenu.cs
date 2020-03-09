using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        Game.Freeze();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void Resume()
    {
        SceneChanger.UnloadPauseMenuAdditive();
        Game.Unfreeze();
    }

    public void OpenMainMenu()
    {
        Singleplayer.Instance.ResetGame();
        SceneChanger.SetMainMenuAsActiveScene();
        Game.Unfreeze();
    }
}
