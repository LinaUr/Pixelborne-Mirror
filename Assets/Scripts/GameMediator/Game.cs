using UnityEngine;

/// <summary></summary>
public class Game : ScriptableObject
{
    /// <summary>Gets or sets the current.</summary>
    /// <value>The current.</value>
    public static IGame Current { get; set; }
    /// <summary>Gets or sets the mode.</summary>
    /// <value>The mode.</value>
    public static Mode Mode { get; set; }

    /// <summary>Pauses this instance.</summary>
    public static void Pause()
    {
        SceneChanger.LoadPauseMenuAdditive();
    }

    /// <summary>Finishes this instance.</summary>
    public static void Finish()
    {
        SceneChanger.SetWinningScreenAsActiveScene();
    }

    /// <summary>Freezes this instance.</summary>
    public static void Freeze()
    {
        Time.timeScale = 0;
        Current.LockPlayerInput(true);
    }

    /// <summary>Unfreezes this instance.</summary>
    public static void Unfreeze()
    {
        Time.timeScale = 1;
        Current.LockPlayerInput(false);
    }

    /// <summary>Swaps the hud symbol.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    public static void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        Current.SwapHudSymbol(gameObject, sprite);
    }
}
