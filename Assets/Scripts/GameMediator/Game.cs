using UnityEngine;

public class Game : ScriptableObject
{
    //private static Game m_instance = null;

    public static Mode CurrentMode { get; set; }
    public static IGame Current { get; set; }

    //public static Game Instance
    //{
    //    get
    //    {
    //        // A ScriptableObject should not be instanciated directly,
    //        // so we use CreateInstance instead.
    //        return m_instance == null ? CreateInstance<Game>() : m_instance;
    //    }
    //    private set { }
    //}

    //public Game()
    //{
    //    m_instance = this;
    //}

    public static void Pause()
    {
        SceneChanger.LoadPauseMenuAdditive();
    }

    public static void HasFinished()
    {
        SceneChanger.SetWinningScreenAsActiveScene();
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
