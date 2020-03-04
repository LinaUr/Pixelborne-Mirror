using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartSingleplayer()
    {
        Singleplayer.Instance.Go();
    }

    public void StartMultiplayer()
    {
        Multiplayer.Instance.Go();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
