using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        GameMediator.Instance.CurrentMode = Mode.MainMenu;
    }

    public void StartSingleplayer()
    {
        Singleplayer.Instance.Start();
    }

    public void StartMultiplayer()
    {
        SceneChanger.SetMultiplayerAsActiveScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
