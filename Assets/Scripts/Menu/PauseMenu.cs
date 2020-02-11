using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        GameMediator.Instance.FreezeGame();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void Resume()
    {
        SceneChanger.UnloadPauseMenuAdditive();
        GameMediator.Instance.UnfreezeGame();
    }

    public void OpenMainMenu()
    {
        SceneChanger.SetMainMenuAsActiveScene();
        GameMediator.Instance.UnfreezeGame();
    }
}
