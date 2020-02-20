using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        GameMediator.Instance.CurrentMode = Mode.MainMenu;
    }

    public void StartSingleplayer()
    {
        Singleplayer.Instance.Go();
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
