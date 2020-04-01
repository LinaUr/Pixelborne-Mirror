using System;
using System.Diagnostics;
using UnityEngine;

// This class implements the fading and hudsymbol swapping for cameras of the game.
public abstract class GameCamera : MonoBehaviour, ICamera
{
    [SerializeField]
    private protected float m_fadeTime = 1500;
    [SerializeField]
    private protected GameObject m_fadeImage;

    private protected FadeMode m_fadeMode = FadeMode.NoFade;
    private protected Stopwatch m_fadeStopwatch = new Stopwatch();

    private protected enum FadeMode
    {
        FadeIn,
        FadeOut,
        NoFade
    }

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
    public void FadeOut()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeOut;
    }

    // This method triggers the fade in animation.
    public void FadeIn()
    {
        m_fadeStopwatch.Start();
        m_fadeMode = FadeMode.FadeIn;
    }

    private protected abstract void FadedOut();

    private protected abstract void FadedIn();

    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        GameObject hudObject = transform.Find($"{gameObject.name}HudSymbol").gameObject;
        SpriteRenderer spriteRenderer = hudObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
