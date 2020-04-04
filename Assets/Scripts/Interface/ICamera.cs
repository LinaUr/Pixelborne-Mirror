﻿// This interface must be implemented by cameras that get used in Singleplayer and Multiplayer mode. 
using UnityEngine;

/// <summary></summary>
public interface ICamera
{
    /// <summary>Fades the out.</summary>
    void FadeOut();
    /// <summary>Fades the in.</summary>
    void FadeIn();
    /// <summary>Swaps the hud symbol.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    void SwapHudSymbol(GameObject gameObject, Sprite sprite);
}