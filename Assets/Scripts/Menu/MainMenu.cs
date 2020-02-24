using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        //Game.CurrentMode = Mode.MainMenu;
    }

    public void StartSingleplayer()
    {
        //Game.CurrentMode = Mode.Singleplayer;
        Singleplayer.Instance.Go();
    }

    public void StartMultiplayer()
    {
        //Game.CurrentMode = Mode.Multiplayer;
        //SceneChanger.SetMultiplayerAsActiveScene();

        Multiplayer.Instance.Go();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
