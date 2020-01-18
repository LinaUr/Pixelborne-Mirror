// This interface must be implemented by cameras that get used in Singleplayer and Multiplayer mode. 
using UnityEngine;

public interface ICamera
{
    void FadeOut();
    void FadeIn();

    void SwapHudSymbol(GameObject gameObject, Sprite sprite);
}