using System;
using System.Diagnostics;
using UnityEngine;

// This class implements the fading and hudsymbol swapping for cameras of the game.
/// <summary></summary>
public abstract class GameCamera : MonoBehaviour, ICamera
{
    /// <summary>The m fade time</summary>
    [SerializeField]
    protected float m_fadeTime = 1500;
    /// <summary>The m fade image</summary>
    [SerializeField]
    protected GameObject m_fadeImage;

    /// <summary>The m fade mode</summary>
    protected FadeMode m_fadeMode = FadeMode.NoFade;
    /// <summary>The m fade stopwatch</summary>
    protected Stopwatch m_fadeStopwatch = new Stopwatch();

    /// <summary></summary>
    protected enum FadeMode
    {
        /// <summary>The fade in</summary>
        FadeIn,
        /// <summary>The fade out</summary>
        FadeOut,
        /// <summary>The no fade</summary>
        NoFade
    }

    /// <summary>Updates this instance.</summary>
    protected virtual void Update()
    {
        Fade();
    }

    // This method implements the fade in and fade out logic.
    private void Fade()
    {
        // Skip function if fade in / fade out was succesfully completed and has not been triggered again.
        if (m_fadeMode == FadeMode.NoFade)
        {
            return;
        }

        long elapsedTime = m_fadeStopwatch.ElapsedMilliseconds;
        // Complete the fade when enough time has passed.
        bool isFadeComplete = elapsedTime >= m_fadeTime;

        Color color = m_fadeImage.GetComponent<SpriteRenderer>().color;

        if (m_fadeMode == FadeMode.FadeOut)
        {
            // Lower the opacity of the fade to black canvas object based on the time passed since the fade to black trigger.
            float percentage = elapsedTime / m_fadeTime;
            color.a = isFadeComplete ? 1.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (isFadeComplete)
            {
                FadeCompleted();
                FadedOut();
            }
        }
        else if (m_fadeMode == FadeMode.FadeIn)
        {
            // Increase the opacity of the fade to black canvas object based on the time passed since fade was triggered. 
            float percentage = 1 - elapsedTime / m_fadeTime;
            color.a = isFadeComplete ? 0.0f : percentage;
            m_fadeImage.GetComponent<SpriteRenderer>().color = color;

            if (isFadeComplete)
            {
                FadeCompleted();
                FadedIn();
            }
        }
        else
        {
            throw new Exception("Error: Wrong fade mode was triggered.");
        }
    }

    // This method resets important variables after a fade finished.
    private void FadeCompleted()
    {
        m_fadeMode = FadeMode.NoFade;
        m_fadeStopwatch.Reset();
    }

    // This method triggers the fade to black animation.
    /// <summary>Fades the out.</summary>
    public void FadeOut()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeOut;
    }

    // This method triggers the fade in animation.
    /// <summary>Fades the in.</summary>
    public void FadeIn()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeIn;
    }

    /// <summary>Fadeds the out.</summary>
    protected abstract void FadedOut();

    /// <summary>Fadeds the in.</summary>
    protected abstract void FadedIn();

    /// <summary>Swaps the hud symbol.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
