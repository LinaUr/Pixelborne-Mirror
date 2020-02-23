using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        Game.Instance.CurrentMode = Mode.MainMenu;
    }

    public void StartSingleplayer()
    {
        Game.Instance.CurrentMode = Mode.Singleplayer;
        Singleplayer.Instance.Go();
    }

    public void StartMultiplayer()
    {
        Game.Instance.CurrentMode = Mode.Multiplayer;
        SceneChanger.SetMultiplayerAsActiveScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
