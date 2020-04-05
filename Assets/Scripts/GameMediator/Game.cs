using UnityEngine;

public class Game : ScriptableObject
{
    public static IGame Current { get; set; }
    public static GameMode Mode { get; set; }

    public static void Pause()
    {
        SceneChanger.LoadPauseMenuAdditive();
    }

    public static void Finish()
    {
        if (Mode == GameMode.Singleplayer)
        {
            SceneChanger.SetMainMenuAsActiveScene();
        }
        else
        {
            SceneChanger.SetWinningScreenAsActiveScene();
        }
    }

    public static void Freeze()
    {
        Time.timeScale = 0;
        Current.LockPlayerInput(true);
    }

    public static void Unfreeze()
    {
        Time.timeScale = 1;
        Current.LockPlayerInput(false);
    }

    public static void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        Current.SwapHudSymbol(gameObject, sprite);
    }
}
